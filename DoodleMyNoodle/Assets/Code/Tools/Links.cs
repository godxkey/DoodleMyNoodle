using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngineX;

public class Links : MonoBehaviour
{
    [MenuItem("Links/Documentation/Project Dependencies", priority = 1)]
    static void OpenProjectDependenciesDocumentation()
    {
        OpenWebPath("https://github.com/CCC-Development/DoodleMyNoodle/tree/master/Documentation/ProjectDependencies/ProjectDependencies.md");
    }

    [MenuItem("Links/Git Hub", priority = 2)]
    static void OpenGitHubPage()
    {
        OpenWebPath("https://github.com/CCC-Development/DoodleMyNoodle");
    }

    [MenuItem("Links/Player Logs", priority = 3)]
    public static void OpenPersistentDataPath()
    {
        OpenDirectory(Application.persistentDataPath);
    }

    [MenuItem("Links/Editor Logs", priority = 4)]
    public static void OpenEditorLogs()
    {
        OpenDirectory(Path.GetDirectoryName(Application.consoleLogPath));
    }

    [MenuItem("Links/Share Documents", priority = 5)]
    public static void OpenGoogleDriveDocuments()
    {
        OpenWebPath("https://drive.google.com/drive/folders/1N6vzdyZMeHAaaaZ2lW3qmKfINyaUKduj");
    }

    private static void OpenWebPath(string path)
    {
        Process.Start(path);
    }

    private static void OpenDirectory(string path)
    {
        path = path.Replace('/', '\\');
        if (Directory.Exists(path))
        {
            Process.Start("explorer.exe", path);
        }
        else
        {
            Log.Warning("Cannot open log file location(" + path + "). Directory was not found.");
        }
    }
}
