// This showcases the CPU usage on the UI thread, in a UI app this would cause small delays and cpu usage spikes.
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Threading;

var ctx = new JoinableTaskContext();
var factory = ctx.Factory;

await Task.Run(() => MainLoop());

async Task MainLoop()
{
    // Both of these will run on the UI thread normally.
    var sw = System.Diagnostics.Stopwatch.StartNew();

    while (true)
    {
        await factory.SwitchToMainThreadAsync();

        sw.Stop();

        await Task.WhenAll(ServiceUIThreadAsync(sw), CpuOnUIThreadAsync(), CpuOnUIThreadAsync());

        sw.Restart();

        await TaskScheduler.Default;
        await Task.Delay(10);
    }
}

[MethodImpl(MethodImplOptions.NoInlining)]
Task ServiceUIThreadAsync(System.Diagnostics.Stopwatch sw)
{
    sw.Stop();
    Console.WriteLine("Took {0}ms", sw.Elapsed);

    // 13ms at most would guarantee 60fps.
    Thread.Sleep(13);

    sw.Restart();
    return Task.CompletedTask;
}

[MethodImpl(MethodImplOptions.NoInlining)]
Task CpuOnUIThreadAsync()
{
    Console.WriteLine("Fake work");
    _ = FakeWork();
    Console.WriteLine("Fake work done");

    Console.WriteLine("Fake work nested");
    _ = FakeWorkWithNesting(10_000);
    Console.WriteLine("Fake work nested done");

    return Task.CompletedTask;
}

[MethodImpl(MethodImplOptions.NoInlining)]
static long FakeWork()
{
    long sum = 0;
    for (int i = 0; i < 1_000_000; ++i)
    {
        sum += (long)i;
    }

    return sum;
}

[MethodImpl(MethodImplOptions.NoInlining)]
static int FakeWorkWithNesting(int depth)
{
    if (depth <= 0)
    {
        return 0;
    }

    int sum = 1 + FakeWorkWithNesting(depth - 1);
    return sum;
}
