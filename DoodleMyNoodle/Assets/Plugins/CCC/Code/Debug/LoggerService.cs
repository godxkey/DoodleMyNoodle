using System;
using System.Collections.Generic;
using System.Globalization;
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
            _loggers = new List<ConfigurableLogger>(_loggerSettings.Length);
            foreach (LoggerSettings setting in _loggerSettings)
            {
                var logger = new ConfigurableLogger(setting.Name);
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
        }

        onComplete(this);
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