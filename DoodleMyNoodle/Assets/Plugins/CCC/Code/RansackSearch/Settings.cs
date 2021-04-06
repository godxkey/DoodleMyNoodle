using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public static partial class RansackSearch
{
    public static class Settings
    {
        private const string KEY = "ransack-exec";

        public static string RansackInstallationPath
        {
            get => EditorPrefs.GetString(KEY, defaultValue: @"C:\Program Files\Mythicsoft\Agent Ransack\AgentRansack.exe");
            set => EditorPrefs.SetString(KEY, value);
        }

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Preferences/Ransack Search Settings", SettingsScope.User)
            {
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    RansackInstallationPath = EditorGUILayout.TextField("Agent Ransack Path", RansackInstallationPath);

                    if(GUILayout.Button("Download Agent Ransack"))
                    {
                        Process.Start("https://www.mythicsoft.com/agentransack/download/");
                    }
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Ransack", "Search", "Agent" })
            };

            return provider;
        }
    }
}
