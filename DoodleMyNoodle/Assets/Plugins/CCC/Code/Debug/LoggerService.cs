using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngineX;

public class LoggerService : MonoCoreService<LoggerService>
{
    [Serializable]
    private class LoggerSettings
    {
        public string Name;
        public ConfigurableLogger.RuleSet FallbackRuleSet;
        public RuleSetEntry[] Rules;

        [Serializable]
        public class RuleSetEntry
        {
            public LogType LogType;
            public ConfigurableLogger.RuleSet RuleSet;
        }
    }

    [SerializeField] private LoggerSettings[] _loggerSettings;

    private List<ConfigurableLogger> _loggers;

    public override void Initialize(Action<ICoreService> onComplete)
    {
        if (_loggerSettings != null)
        {
            string unformattedOutputPath = GetUnformatedOutputPath(out string errorLog);

            if (string.IsNullOrEmpty(unformattedOutputPath))
            {
                UnityEngine.Debug.LogError(errorLog);
            }
            else
            {
                _loggers = new List<ConfigurableLogger>(_loggerSettings.Length);
                foreach (LoggerSettings setting in _loggerSettings)
                {
                    var logger = new ConfigurableLogger(string.Format(unformattedOutputPath, setting.Name));
                    logger.FallbackRuleSet = setting.FallbackRuleSet;

                    if (setting.Rules != null)
                    {
                        foreach (LoggerSettings.RuleSetEntry ruleSetSetting in setting.Rules)
                        {
                            logger.Rules[ruleSetSetting.LogType] = ruleSetSetting.RuleSet;
                        }
                    }

                    _loggers.Add(logger);
                }

                if (!string.IsNullOrEmpty(errorLog))
                    UnityEngine.Debug.LogError(errorLog);
            }
        }

        onComplete(this);
    }

    private string GetUnformatedOutputPath(out string errorLog)
    {
        errorLog = "";
        string unityLogPath = Application.consoleLogPath;

        if (unityLogPath.Contains("\r"))
        {
            unityLogPath = unityLogPath.Replace("\r", "");
            errorLog += "Application.consoleLogPath contains an illegal \\r character. Removing.\n\n";
        }

        try
        {
            return $"{Path.GetDirectoryName(unityLogPath)}" +
                $"\\{Path.GetFileNameWithoutExtension(unityLogPath)}" +
                $"({{0}})" +
                $"{(Application.isEditor ? "" : Process.GetCurrentProcess().Id.ToString())}" +
                $".log";
        }
        catch (Exception e)
        {
            errorLog += $"Failed to GetUnformatedOutputPath: {e.Message}\n{e.StackTrace}.";
            return "";
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_loggers != null)
        {
            foreach (var logger in _loggers)
            {
                logger.Dispose();
            }
        }
    }
}