using System.IO;
using UnityEditor;
using UnityEngine;

public static class EditorHelper
{
    public static void DrawScript_ScriptableObject<T>(Object target) where T: ScriptableObject
    {
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((T)target), typeof(T), false);
        GUI.enabled = true;
    }

    public static void DrawScript_MonoBehaviour<T>(Object target) where T : MonoBehaviour
    {
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((T)target), typeof(T), false);
        GUI.enabled = true;
    }

    public static void DrawOpenFileLocation(string path)
    {
        if (GUILayout.Button("Open File Location"))
        {
            path = path.Replace('/', '\\');

            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
        }
    }
}
