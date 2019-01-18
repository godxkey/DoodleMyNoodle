using System;
using UnityEngine;

public static class ActionExtensions
{
    static public void SafeInvoke<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2, SafeInvokeLogMode logMode = SafeInvokeLogMode.Error)
    {
        try
        {
            action.Invoke(t1, t2);
        }
        catch (Exception e)
        {
            Log(e, logMode);
        }
    }
    static public void SafeInvoke<T>(this Action<T> action, T t, SafeInvokeLogMode logMode = SafeInvokeLogMode.Error)
    {
        try
        {
            action.Invoke(t);
        }
        catch (Exception e)
        {
            Log(e, logMode);
        }
    }
    static public void SafeInvoke(this Action action, SafeInvokeLogMode logMode = SafeInvokeLogMode.Error)
    {
        try
        {
            action.Invoke();
        }
        catch (Exception e)
        {
            Log(e, logMode);
        }
    }
    static public void SafeInvokeInEditor<T1, T2>(this Action<T1, T2> action, T1 t1, T2 t2, SafeInvokeLogMode logMode = SafeInvokeLogMode.Error)
    {
#if UNITY_EDITOR
        try
        {
#endif
            action.Invoke(t1, t2);
#if UNITY_EDITOR
        }
        catch (Exception e)
        {
            Log(e, logMode);
        }
#endif
    }
    static public void SafeInvokeInEditor<T>(this Action<T> action, T t, SafeInvokeLogMode logMode = SafeInvokeLogMode.Error)
    {
#if UNITY_EDITOR
        try
        {
#endif
            action.Invoke(t);
#if UNITY_EDITOR
        }
        catch (Exception e)
        {
            Log(e, logMode);
        }
#endif
    }
    static public void SafeInvokeInEditor(this Action action, SafeInvokeLogMode logMode = SafeInvokeLogMode.Error)
    {
#if UNITY_EDITOR
        try
        {
#endif
            action.Invoke();
#if UNITY_EDITOR
        }
        catch (Exception e)
        {
            Log(e, logMode);
        }
#endif
    }

    static private void Log(Exception exception, SafeInvokeLogMode logMode)
    {
        switch (logMode)
        {
            case SafeInvokeLogMode.Warning:
                Debug.LogWarning("Error in SafeInvoke: " + exception.Message);
                break;
            case SafeInvokeLogMode.Error:
                Debug.LogError("Error in SafeInvoke: " + exception.Message);
                break;
        }
    }
}

public enum SafeInvokeLogMode
{
    NoLog,
    Warning,
    Error
}
