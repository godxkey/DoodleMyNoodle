using System;
using System.Threading.Tasks;

public static class AsyncUtility
{
    public static void Run(Func<Task> asyncAction)
    {
        asyncAction();
    }
}
