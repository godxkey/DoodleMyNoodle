using System.Threading;

public static class ThreadUtility
{
    public static bool IsMainThread => Thread.CurrentThread.ManagedThreadId == MainThreadService.MainThreadId;
}