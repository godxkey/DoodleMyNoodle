using CCC.Debug;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using UnityX;

public class TestScript : MonoBehaviour
{
    public ModelSpaceTimeDebugger model = new ModelSpaceTimeDebugger();

    private void Start()
    {
        //new ViewSpaceTimeDebugger(model);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Application.logMessageReceived -= Application_logMessageReceived;
            Application.logMessageReceived += Application_logMessageReceived;
            Application.logMessageReceivedThreaded -= Application_logMessageReceivedThreaded;
            Application.logMessageReceivedThreaded += Application_logMessageReceivedThreaded;
            UnityEngine.Debug.Log("hello");
            Log.Info("hello");

            new Thread(() => { Thread.Sleep(100); UnityEngine.Debug.Log("threaded hello"); }).Start();
        }
    }

    private void Application_logMessageReceivedThreaded(string condition, string stackTrace, LogType type)
    {
        UnityEngine.Debug.Log("Application_logMessageReceivedThreaded: " + condition);
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        UnityEngine.Debug.Log("Application_logMessageReceived: " + condition);
    }
}


public static class SpaceTimeDebugger
{
    private class Stream
    {
        public readonly string Name;
        public List<LoggedValue> Values = new List<LoggedValue>();

        public Stream(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }

    private struct LoggedValue
    {
        public long Tick;
        public float Value;
    }

    private static Dictionary<string, Stream> s_streams = new Dictionary<string, Stream>();
    private static Stopwatch s_stopwatch;

    public static void Log(string streamName, float value)
    {

        LoggedValue loggedValue = new LoggedValue()
        {
            Tick = GetCurrentTicks(),
            Value = value
        };

        var stream = GetOrCreateStream(streamName);
        stream.Values.Add(loggedValue);
    }

    private static long GetCurrentTicks()
    {
        if (s_stopwatch == null)
        {
            s_stopwatch = new Stopwatch();
            s_stopwatch.Start();
        }

        return s_stopwatch.Elapsed.Ticks;
    }

    private static Stream GetOrCreateStream(string name)
    {
        if (!s_streams.TryGetValue(name, out Stream s))
        {
            s = new Stream(name);
        }

        return s;
    }
}
