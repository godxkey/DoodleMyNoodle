using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class VariousShortcuts
{
    [MenuItem("Tools/Shortcuts/Open Presistent Data Path #&p")]
    public static void OpenPersistentDataPath()
    {
        string path = Application.persistentDataPath;
        path = path.Replace('/', '\\');
        if (Directory.Exists(path))
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
        else
        {
            DebugService.LogWarning("Cannot open log file location(" + path + "). Directory was not found.");
        }
    }
}
