using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngineX;

public static partial class RansackSearch
{
    public class RansackQuery : IDisposable
    {
        public class ResultEntry
        {
            public string ProjectDirectory;
            public string File;
            public List<string> Lines = new List<string>();

            public string CompleteProjectPath => ProjectDirectory + File;
            public string AssetPath => "Assets/" + ProjectDirectory + File;
        }

        private class CSVLine
        {
            public string Directory;
            public string File;
            public string Size;
            public string Type;
            public string Date1;
            public string Date2;
            public string Date3;
            public string Count;
            public string LineNumber;
            public string LineContent;

            public static readonly FieldInfo[] FieldInfos = typeof(CSVLine).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            public void SetAllFields(List<string> fieldStrings)
            {
                for (int i = 0; i < FieldInfos.Length; i++)
                {
                    FieldInfos[i].SetValue(this, fieldStrings[i]);
                }
            }
        }

        public class Result
        {
            public List<ResultEntry> ResultEntries = new List<ResultEntry>();
        }

        private Process _process;
        private string _outputFile;
        private bool _disposed = false;

        public string Search { get; private set; }

        public RansackQuery(string search, string fileNames)
        {
            Search = search;
            var args = new List<string>();
            _outputFile = FileUtil.GetUniqueTempPathInProject();

            args.Add($"-o {_outputFile}");
            args.Add($"-c {search}");
            args.Add($"-d {Application.dataPath}");
            args.Add($"-ofc");
            if (!string.IsNullOrEmpty(fileNames))
            {
                args.Add($"-f \"{fileNames}\"");
                args.Add($"-fed");
            }
            _process = StartRansack(string.Join(" ", args));
        }

        public static void OpenRansackExternal(string search, string fileNames)
        {
            var args = new List<string>();

            args.Add($"-c {search}");
            args.Add($"-d {Application.dataPath}");
            if (!string.IsNullOrEmpty(fileNames))
            {
                args.Add($"-f \"{fileNames}\"");
                args.Add($"-fed");
            }

            StartRansack(string.Join(" ", args));
        }

        private static Process StartRansack(string args)
        {
            try
            {
                return Process.Start(Settings.RansackInstallationPath, string.Join(" ", args));
            }
            catch
            {
                EditorUtility.DisplayDialog("Ransack Search Error", $"Could not find Agent Ransack installation at:" +
                    $"\n{Settings.RansackInstallationPath}" +
                    $"\n\n The path can be edited in the Preferences window.", "Ok");
            }
            return null;
        }

        public Result Read()
        {
            Result result = new Result();

            if (!File.Exists(_outputFile))
                return result;

            FileStream logFileStream = new FileStream(_outputFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader logFileReader = new StreamReader(logFileStream);

            try
            {
                CSVLine csvLine = new CSVLine();
                List<string> fields = new List<string>();
                while (!logFileReader.EndOfStream)
                {
                    string line = logFileReader.ReadLine();

                    ParseLine(line, fields);

                    csvLine.SetAllFields(fields);
                    csvLine.Directory = csvLine.Directory.RemoveFirst(Application.dataPath + "\\");
                    csvLine.Directory = csvLine.Directory.Replace("\\", "/");

                    bool sameAsPrevious = result.ResultEntries.Count > 0 && result.ResultEntries.Last().ProjectDirectory == csvLine.Directory && result.ResultEntries.Last().File == csvLine.File;

                    ResultEntry entry;
                    if (!sameAsPrevious)
                    {
                        entry = new ResultEntry();
                        entry.ProjectDirectory = csvLine.Directory;
                        entry.File = csvLine.File;
                    }
                    else
                    {
                        entry = result.ResultEntries.Last();
                    }

                    entry.Lines.Add($"{csvLine.LineNumber}    {csvLine.LineContent}");

                    result.ResultEntries.Add(entry);
                }
            }
            finally
            {
                logFileReader.Close();
                logFileStream.Close();
            }

            return result;
        }

        private static void ParseLine(string line, List<string> fields)
        {
            fields.Clear();

            int begin = 0;
            bool inQuote = false;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                {
                    inQuote = !inQuote;
                }

                if (line[i] == ',' && !inQuote)
                {
                    split(i);
                }
            }

            split(line.Length);

            void split(int i)
            {
                int length = i - begin;
                if (length > 0 && begin < line.Length)
                {
                    string sub = line.Substring(begin, length);
                    if (sub[0] == '"' && sub.Length > 2) // remove surrounding quotes
                    {
                        sub = sub.Substring(1, sub.Length - 2);
                    }
                    fields.Add(sub);
                    begin = i + 1;
                }
            }
        }

        public bool IsDone()
        {
            return _process != null && _process.HasExited;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (_process != null && !_process.HasExited)
            {
                _process.Kill();
            }

            if (File.Exists(_outputFile))
            {
                File.Delete(_outputFile);
            }
        }
    }
}
