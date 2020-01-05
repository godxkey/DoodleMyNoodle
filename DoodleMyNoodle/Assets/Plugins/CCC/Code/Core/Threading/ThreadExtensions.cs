using System.Collections;
using System.Threading;

public static class ThreadExtensions
{
    public static IEnumerator StartAndWaitForComplete(this Thread t)
    {
        t.Start();
        while (t.IsAlive)
        {
            yield return null;
        }
    }
}