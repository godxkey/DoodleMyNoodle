using CCC.IO;
using System;
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
    private FileWriter _fileWriter;
    private ConcurrentQueue<string> _pendingLogs = new ConcurrentQueue<string>();
    private ConcurrentBag<StringBuilder> _stringBuilders = new ConcurrentBag<StringBuilder>();
    private static string s_nowString;
    private static double s_nextNowTime = -1;

    public ConfigurableLogger(string name)
    {
        if (Log.Enabled)
        {
            string unityLogPath = Application.consoleLogPath;//

            string path =
                $"{Path.GetDirectoryName(unityLogPath)}" +
                $"\\{Path.GetFileNameWithoutExtension(unityLogPath)}" +
                $"({name})" +
                $"{(Application.isEditor ? "" : Process.GetCurrentProcess().Id.ToString())}" +
                $".txt";

            _fileWriter = new FileWriter(_pendingLogs, path);
            Log.Internals.LogMessageReceivedThreaded += OnLogReceived;
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

        str.Append("[");
        str.Append(GetNowString());
        str.Append("] ");

        str.Append("[F");
        str.Append(Time.frameCount);
        str.Append("] ");

        if (channelId != -1)
        {
            str.Append("[");
            str.Append(Log.GetChannel(channelId).Name);
            str.Append("] ");
        }

        str.Append(' ');
        str.AppendLine(condition);

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

    private static double GetAppTime()
    {
#if UNITY_EDITOR
        return UnityEditor.EditorApplication.timeSinceStartup;
#else
        return Time.unscaledTime;
#endif
    }

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