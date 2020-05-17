using CCC.Debug;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using Unity.Collections;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    public ModelSpaceTimeDebugger model = new ModelSpaceTimeDebugger();

    private void Start()
    {
        //new ViewSpaceTimeDebugger(model);
    }

    private void Update()
    {
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
        if(!s_streams.TryGetValue(name, out Stream s))
        {
            s = new Stream(name);
        }

        return s;
    }
}
