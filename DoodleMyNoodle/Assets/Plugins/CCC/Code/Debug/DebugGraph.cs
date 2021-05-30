using CCC.Debug;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGraph
{
    private static bool s_init = false;
    [RuntimeInitializeOnLoadMethod()]
    static void OnRuntimeMethodLoad() // Executed after scene is loaded and game is running
    {
        if (!s_init)
        {
            s_init = true;
            Init();
        }
    }

    private static readonly Color[] s_colors = new Color[]
    {
        Color.magenta,
        Color.red,
        Color.blue,
        Color.green,
        Color.cyan,
        Color.white,
        Color.black,
        Color.grey,
    };

    const float TIME_RANGE = 5;
    private static GraphDrawer s_graphDrawer = new GraphDrawer();
    private static bool s_displayGraph = false;

    class LoggedCurve
    {
        public string Name;
        public List<Vector2> Points = new List<Vector2>();
    }

    struct LoggedTime
    {
        public Color Color;
        public string Label;
        public float Time;
    }

    private static List<LoggedCurve> s_loggedCurves = new List<LoggedCurve>();
    private static List<LoggedTime> s_loggedTimes = new List<LoggedTime>();
    private static List<GraphDrawer.Curve> s_curveCache = new List<GraphDrawer.Curve>();

    private static void Init()
    {
        s_graphDrawer.AutoZoomVertical = true;
        s_graphDrawer.AutoZoomHorizontal = false;
    }

    //[Updater.StaticUpdateMethod(UpdateType.Update)]
    //private static void OnUpdate()
    //{
    //    if (Input.GetKeyDown(KeyCode.F3))
    //    {
    //        s_displayGraph = !s_displayGraph;
    //    }
    //}

    [Updater.StaticUpdateMethod(UpdateType.GUI)]
    private static void OnGUI()
    {
        if (!s_displayGraph)
            return;

        int curveI = 0;
        GraphDrawer.Curve GetCurve()
        {
            if (curveI == s_curveCache.Count)
                s_curveCache.Add(new GraphDrawer.Curve());
            return s_curveCache[curveI];
        }


        for (int i = 0; i < s_loggedCurves.Count; i++)
        {
            LoggedCurve loggedCurve = s_loggedCurves[i];

            // remove all points that are older than TIME_RANGE
            for (int p = 0; p < loggedCurve.Points.Count - 1; p++)
            {
                if (Time.time - loggedCurve.Points[p].x > TIME_RANGE &&
                    Time.time - loggedCurve.Points[p + 1].x > TIME_RANGE)
                {
                    loggedCurve.Points.RemoveAt(p);
                    p--;
                }
            }

            if (loggedCurve.Points.Count == 0)
            {
                s_loggedCurves.RemoveAt(i);
                i--;
            }
            else
            {
                GraphDrawer.Curve coloredCurve = GetCurve();

                coloredCurve.Positions.Clear();
                coloredCurve.Positions.AddRange(loggedCurve.Points);
            }
        }

        s_graphDrawer.ScreenDisplayRect.xMin = Time.time - TIME_RANGE;
        s_graphDrawer.ScreenDisplayRect.xMax = Time.time;
        s_graphDrawer.Draw();
    }

    public static void Log(string curveName, float value)
    {
        LoggedCurve curve = null;
        for (int i = 0; i < s_loggedCurves.Count; i++)
        {
            if (s_loggedCurves[i].Name == curveName)
            {
                curve = s_loggedCurves[i];
            }
        }

        if (curve == null)
            s_loggedCurves.Add(curve = new LoggedCurve());

        curve.Points.Add(new Vector2(Time.time, value));
    }

    public static void LogTime(string label = "")
    {
        LogTime(Color.gray, Time.time, label);
    }
    public static void LogTime(Color color, string label = "")
    {
        LogTime(color, Time.time, label);
    }
    public static void LogTime(float time, string label = "")
    {
        LogTime(Color.gray, time, label);
    }
    public static void LogTime(Color color, float time, string label = "")
    {
        s_loggedTimes.Add(new LoggedTime() { Color = color, Label = label, Time = time });
    }
}
