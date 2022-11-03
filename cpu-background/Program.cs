// Run infinite loops in a few background threads.

using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Threading;

SynchronizationContext.SetSynchronizationContext(new SingleThreadedSynchronizationContext());
Thread.CurrentThread.Name = "Main Thread";

for (int i = 0; i < 3; ++i)
{
    var tasks = Enumerable.Repeat(0, 5).Select(x => Task.Run(() => InfiniteLoopAsync()));

    await Task.WhenAll(tasks);
}

[MethodImpl(MethodImplOptions.NoInlining)]
static async Task InfiniteLoopAsync()
{
    string file = Path.GetTempFileName();

    // Simulate some delay to avoid 100% CPU usage.
    long sum = 0;

    while (true)
    {
        await Task.Delay(1);

        sum += FakeWork();
        sum += FakeWork();

        // Do some IO.
        File.WriteAllText(file, sum.ToString());
    }
}

[MethodImpl(MethodImplOptions.NoInlining)]
static int FakeWork()
{
    int sum = 0;
    for (int i = 0; i < 1_000_000; ++i)
    {
        sum += i;
    }

    return sum;
}
