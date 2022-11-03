// A background thread takes a lock for 1 second, and we'll see the UI thread stalling waiting for it.
using Microsoft.VisualStudio.Threading;

SynchronizationContext.SetSynchronizationContext(new SingleThreadedSynchronizationContext());
Thread.CurrentThread.Name = "Main Thread";

var mutex = new SemaphoreSlim(1, 1);

// Take the lock and release it for a bit every second.
await Task.Run(async () =>
{
    while (true)
    {
        using var _ = await mutex.LockAsync();

        var delay = Random.Shared.Next(10, 100);
        await Task.Delay(delay);
    }
});

var sw = System.Diagnostics.Stopwatch.StartNew();
while (true)
{
    sw.Restart();
    using var _ = await mutex.LockAsync();
    sw.Stop();

    Console.WriteLine("Acquiring mutex took: {0} ms", sw.Elapsed);
    await Task.Delay(100);
}

internal static partial class SemaphoreSlimExtensions
{
    internal static async Task<SemaphoreSlimLocker> LockAsync(this SemaphoreSlim @lock, CancellationToken cancellationToken = default)
    {
        await @lock.WaitAsync(cancellationToken).ConfigureAwait(false);

        return new SemaphoreSlimLocker(@lock);
    }

    internal static SemaphoreSlimLocker Lock(this SemaphoreSlim @lock, CancellationToken cancellationToken = default)
    {
        @lock.Wait(cancellationToken);

        return new SemaphoreSlimLocker(@lock);
    }

    internal readonly struct SemaphoreSlimLocker : IDisposable
    {
        readonly SemaphoreSlim _lock;

        internal SemaphoreSlimLocker(SemaphoreSlim @lock)
        {
            _lock = @lock;
        }

        public void Dispose() => _lock.Release();
    }
}
