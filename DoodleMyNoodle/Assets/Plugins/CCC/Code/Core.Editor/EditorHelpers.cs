using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class EditorHelpers
{
    public static void DrawScript_ScriptableObject<T>(Object target) where T : ScriptableObject
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
            OpenDirectoryWithExplorer(path);
        }
    }

    public static bool OpenDirectoryWithExplorer(string path)
    {
        path = path.Replace('/', '\\');

        if (Directory.Exists(path))
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
            return true;
        }
        return false;
    }

    private static PropertyInfo _cachedInspectorModeInfo;

    public static long GetSceneObjectLocalFileIdentifier(UnityEngine.Object obj)
    {
        long idInFile = 0;
        SerializedObject serialize = new SerializedObject(obj);

        if (_cachedInspectorModeInfo == null)
        {
            _cachedInspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        _cachedInspectorModeInfo.SetValue(serialize, InspectorMode.Debug, null);

        SerializedProperty localIdProp = serialize.FindProperty("m_LocalIdentfierInFile");
        idInFile = localIdProp.longValue;

        return idInFile;
    }
}
