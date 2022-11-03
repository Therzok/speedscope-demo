// Allocate in a loop, to showcase GC frames are not marked.

using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.Threading;

SynchronizationContext.SetSynchronizationContext(new SingleThreadedSynchronizationContext());
Thread.CurrentThread.Name = "Main Thread";

while (true)
{
    for (int i = 0; i < 100_000; ++i)
    {
        _ = Allocate().Sum();
    }
}

[MethodImpl(MethodImplOptions.NoInlining)]
static int[] AllocateArray()
{
    return new int[10_000_000];
}

[MethodImpl(MethodImplOptions.NoInlining)]
static int[] Allocate()
{
    var array = AllocateArray();
    Array.Fill(array, 1);

    return array;
}
