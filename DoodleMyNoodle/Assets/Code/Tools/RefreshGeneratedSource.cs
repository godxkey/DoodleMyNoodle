using System.IO;
using UnityEditor;
using UnityEngine;

public static class RefreshGeneratedSource
{
    [MenuItem("Tools/RefreshGeneratedSource %&r")]
    public static void ResetGeneratedCode()
    {
        var generatedCodePath = Path.Combine(Application.dataPath, "../", "Temp/GeneratedCode");
        var resolvedPath = Path.GetFullPath(generatedCodePath);
        Debug.Log($"Removing: {resolvedPath}");
        if (Directory.Exists(resolvedPath))
            Directory.Delete(resolvedPath, true);

        EditorUtility.RequestScriptReload();
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
    }
}