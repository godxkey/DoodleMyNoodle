using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngineX;

public static class SpaceTimeDebugger
{
    internal class TimedElementImpl
    {
        public int Id;
        public string Name;
        public Color Color;

        public List<double> StopwatchTimes = new List<double>();

        protected int FindTimeIndex(double time, int roundingAdjustment)
        {
            int index = StopwatchTimes.BinarySearch(time);
            if (index < 0)
            {
                index = Mathf.Abs(index) + roundingAdjustment;
            }

            return Mathf.Clamp(index, 0, StopwatchTimes.Count - 1);
        }

        protected (int indexBegin, int indexEnd) FindTimeIndices(double timeBegin, double timeEnd)
        {
            return (FindTimeIndex(timeBegin, roundingAdjustment: -1), FindTimeIndex(timeEnd, roundingAdjustment: 0));
        }
    }

    internal class ClockImpl : TimedElementImpl
    {
        public void Tick()
        {
            StopwatchTimes.Add(Stopwatch.Elapsed.TotalSeconds);
        }

        public void GetValues(double timeBegin, double timeEnd, List<double> result)
        {
            int indexBegin = FindTimeIndex(timeBegin, roundingAdjustment: -1);
            int indexEnd = FindTimeIndex(timeEnd, roundingAdjustment: 0);

            result.Clear();
            for (int i = indexBegin; i < indexEnd; i++)
            {
                result.Add((StopwatchTimes[i]));
            }
        }
    }

    internal class StreamImpl
    {
        public int Id;
        public string Name;
        public Color Color;

        public List<double> StopwatchTimes = new List<double>();
        public List<float> Values = new List<float>();

        public void Log(float value)
        {
            StopwatchTimes.Add(Stopwatch.Elapsed.TotalSeconds);
            Values.Add(value);
        }

        public void GetValues(double timeBegin, double timeEnd, List<(double time, float value)> result)
        {
            int indexBegin = FindTimeIndex(timeBegin, roundingAdjustment: -1);
            int indexEnd = FindTimeIndex(timeEnd, roundingAdjustment: 0);

            result.Clear();
            for (int i = indexBegin; i < indexEnd; i++)
            {
                result.Add((StopwatchTimes[i], Values[i]));
            }
        }

        private int FindTimeIndex(double time, int roundingAdjustment)
        {
            int index = StopwatchTimes.BinarySearch(time);
            if (index < 0)
            {
                index = Mathf.Abs(index) + roundingAdjustment;
            }

            return Mathf.Clamp(index, 0, StopwatchTimes.Count - 1);
        }
    }

    public struct Stream : IDisposable
    {
        internal int Id;

        public void Dispose()
        {
            Streams.Remove(Id);
        }

        public void Log(float value)
        {
            Streams[Id].Log(value);
        }
    }

    public struct Clock : IDisposable
    {
        internal int Id;

        public void Dispose()
        {
            Clocks.Remove(Id);
        }

        public void Tick()
        {
            Clocks[Id].Tick();
        }
    }

    internal static Stopwatch Stopwatch;
    internal static Dictionary<int, StreamImpl> Streams = new Dictionary<int, StreamImpl>();
    internal static Dictionary<int, ClockImpl> Clocks = new Dictionary<int, ClockImpl>();

    private static bool s_initialized = false;
    private static int s_nextId = 1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)] // initializes in build & playmode
    private static void Initialize()
    {
        if (s_initialized)
            return;
        s_initialized = true;
        Stopwatch = Stopwatch.StartNew();
    }

    public static Clock CreateClock(string name, Color color)
    {
        var clock = new ClockImpl()
        {
            Color = color,
            Name = name,
            Id = s_nextId++,
        };
        Clocks[clock.Id] = clock;
        return new Clock() { Id = clock.Id };
    }

    public static Stream CreateStream(string name, Color color)
    {
        var stream = new StreamImpl()
        {
            Color = color,
            Name = name,
            Id = s_nextId++,
        };
        Streams[stream.Id] = stream;
        return new Stream() { Id = stream.Id };
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
    public class AutoLog : Attribute
    {
        // TODO, search for all static members with this attribute and log them at every clock
    }
}

