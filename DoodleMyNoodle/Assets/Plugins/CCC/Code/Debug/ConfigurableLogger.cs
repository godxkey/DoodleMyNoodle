using CCC.InspectorDisplay;
using CCC.IO;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngineX;

public class ConfigurableLogger : IDisposable
{
    [Serializable]
    public class RuleSet
    {
        public bool OutputLogType;
        public bool OutputTime;
        public bool OutputFrameCount;
        public bool OutputLogChannel;
        public bool OutputStackTrace;
        public int StackTraceLineCount;

        public static RuleSet Default => new RuleSet()
        {
            OutputLogType = true,
            OutputTime = true,
            OutputFrameCount = true,
            OutputLogChannel = true,
            OutputStackTrace = true,
            StackTraceLineCount = 9999
        };
    }

    private FileWriter _fileWriter;
    private ConcurrentQueue<string> _pendingLogs = new ConcurrentQueue<string>();
    private ConcurrentBag<StringBuilder> _stringBuilders = new ConcurrentBag<StringBuilder>();
    private static string s_nowString;
    private static double s_nextNowTime = -1;

    public bool IndentStackTrace { get; set; } = true;
    public RuleSet FallbackRuleSet { get; set; } = RuleSet.Default;
    public Dictionary<LogType, RuleSet> Rules { get; } = new Dictionary<LogType, RuleSet>();

    public ConfigurableLogger(string name)
    {
        if (Log.Enabled)
        {
            string unityLogPath = Application.consoleLogPath;

            string s = "";
            for (int i = 0; i < unityLogPath.Length; i++)
            {
                s += $"char {i}: '{unityLogPath[i]}'   value:{Convert.ToUInt64(unityLogPath[i])}\n\n";
            }

            UnityEngine.Debug.LogError($"path: '{unityLogPath}'   hash:{unityLogPath.GetHashCode()}\n\n" + s);

            string path =
                $"{Path.GetDirectoryName(unityLogPath)}" +
                $"\\{Path.GetFileNameWithoutExtension(unityLogPath)}" +
                $"({name})" +
                $"{(Application.isEditor ? "" : Process.GetCurrentProcess().Id.ToString())}" +
                $".log";

            _fileWriter = new FileWriter(_pendingLogs, path);
            Log.Internals.LogMessageReceivedThreaded += OnLogReceived;

            StartUpdateIfNeeded();
        }
    }

    public void Dispose()
    {
        if (Log.Enabled)
        {
            Log.Internals.LogMessageReceivedThreaded -= OnLogReceived;
            _fileWriter.Dispose();
        }
    }

    private void OnLogReceived(int channelId, string condition, string stackTrace, LogType logType)
    {
        if (!_stringBuilders.TryTake(out StringBuilder str))
        {
            str = new StringBuilder();
        }

        if (!Rules.TryGetValue(logType, out RuleSet ruleSet))
        {
            ruleSet = FallbackRuleSet;
        }

        if (ruleSet.OutputLogType)
        {
            str.Append("[");
            switch (logType)
            {
                case LogType.Error:
                    str.Append("ERR");
                    break;
                case LogType.Assert:
                    str.Append("ASSERT");
                    break;
                case LogType.Warning:
                    str.Append("WARN");
                    break;
                case LogType.Log:
                    str.Append("INFO");
                    break;
                case LogType.Exception:
                    str.Append("EXCEPTION");
                    break;
            }
            str.Append("] ");
        }

        if (ruleSet.OutputTime)
        {
            str.Append("[");
            str.Append(GetNowString());
            str.Append("] ");
        }

        if (ruleSet.OutputFrameCount)
        {
            str.Append("[F");
            str.Append(GetFrameCount());
            str.Append("] ");
        }

        if (ruleSet.OutputLogChannel)
        {
            if (channelId != -1)
            {
                str.Append("[");
                str.Append(Log.GetChannel(channelId).Name);
                str.Append("] ");
            }
        }

        str.Append(' ');
        str.AppendLine(condition);

        if (ruleSet.OutputStackTrace)
        {
            int indexOfLastLine = 0;
            int lineCount = 0;
            for (int i = 0; i < stackTrace.Length; i++)
            {
                if (stackTrace[i] == '\n' || stackTrace[i] == '\r')
                {
                    indexOfLastLine = i;
                    lineCount++;

                    if (lineCount >= ruleSet.StackTraceLineCount)
                    {
                        break;
                    }
                }
            }

            string shortenedStackTrace = stackTrace.Remove(indexOfLastLine, stackTrace.Length - indexOfLastLine);

            if (!string.IsNullOrEmpty(shortenedStackTrace))
            {
                if (IndentStackTrace)
                {
                    string[] stackTracePieces = shortenedStackTrace.Split('\n', '\r');
                    foreach (var item in stackTracePieces)
                    {
                        str.Append("    ");
                        str.AppendLine(item);
                    }
                }
                else
                {
                    str.AppendLine(shortenedStackTrace);
                }
            }

        }

        _pendingLogs.Enqueue(str.ToString());

        str.Clear();
        _stringBuilders.Add(str);
    }

    private static string GetNowString()
    {
        double appTime = GetAppTime();

        if (appTime > s_nextNowTime)
        {
            DateTime now = DateTime.Now;
            s_nowString = now.ToShortDateString() + "/" + now.ToLongTimeString();
            s_nextNowTime = appTime + 1;
        }

        return s_nowString;
    }

    #region Time management
    private static double s_time;
    private static readonly object s_timeLock = new object();
    private static int s_frameCount;
    private static readonly object s_frameCountLock = new object();
    private static readonly WaitForEndOfFrame s_waitEndOfFrame = new WaitForEndOfFrame();
    private static bool s_updating = false;

    private static void StartUpdateIfNeeded()
    {
        if (!s_updating)
        {
            s_updating = true;
            CoroutineLauncherService.Instance.StartCoroutine(Update());
        }
    }

    private static IEnumerator Update()
    {
        while (true)
        {
            lock (s_timeLock)
            {
#if UNITY_EDITOR
                s_time = UnityEditor.EditorApplication.timeSinceStartup;
#else
                s_time = Time.unscaledTime;
#endif
            }

            lock (s_frameCountLock)
            {
                s_frameCount = Time.frameCount;
            }

            yield return s_waitEndOfFrame;
        }
    }

    private static double GetAppTime()
    {
        lock (s_timeLock)
        {
            return s_time;
        }
    }

    private static int GetFrameCount()
    {
        lock (s_frameCountLock)
        {
            return s_frameCount;
        }
    }
    #endregion

    private class FileWriter : IDisposable
    {
        private ConcurrentQueue<string> _pendingLogs = new ConcurrentQueue<string>();
        private Thread _thread;
        private bool _disposed;
        private bool _sleeping;
        private string _logFilePath;
        private List<string> _writeBuffer = new List<string>();

        public FileWriter(ConcurrentQueue<string> pendingLogs, string logFilePath)
        {
            _logFilePath = logFilePath;
            _pendingLogs = pendingLogs ?? throw new ArgumentNullException(nameof(pendingLogs));

            if (File.Exists(_logFilePath))
            {
                DateTime lastWriteTime = File.GetLastWriteTimeUtc(_logFilePath);
                DateTime appOpenTime = DateTime.UtcNow - TimeSpan.FromSeconds(GetAppTime());

                // clear log file if it used for previous session
                if (appOpenTime > lastWriteTime)
                {
                    string prevPath = PathX.RemoveExtension(_logFilePath) + "-prev" + Path.GetExtension(_logFilePath);
                    if (File.Exists(prevPath))
                    {
                        File.Delete(prevPath);
                    }
                    File.Move(_logFilePath, prevPath);
                }
            }

            // launch 'write loop' thread
            _thread = new Thread(ThreadedWriteLoop);
            _thread.Start();
        }

        public void Dispose()
        {
            _disposed = true;

            if (!_sleeping)
            {
                _thread.Join(); // join thread to complete any ongoing write
            }
            else
            {
                _thread.Abort();
            }

            Write();
        }

        private void ThreadedWriteLoop()
        {
            try
            {
                while (!_disposed)
                {
                    Write();

                    _sleeping = true;
                    if (!_disposed)
                    {
                        Thread.Sleep(1000);
                    }
                    _sleeping = false;
                }
            }
            catch (ThreadAbortException)
            {
            }
        }

        private void Write()
        {
            while (_pendingLogs.TryDequeue(out string log))
            {
                _writeBuffer.Add(log);
            }

            if (_writeBuffer.Count > 0)
            {
                try
                {
                    File.AppendAllLines(_logFilePath, _writeBuffer, Encoding.Default);
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to write to log file {_logFilePath}: {e.Message}");
                }
            }

            _writeBuffer.Clear();
        }
    }
}