using UnityEngine;
using UnityEditor;

public static class SetDirtyMenuItem
{
    [MenuItem("Assets/Set Dirty")]
    public static void SetDirty()
    {
        foreach (var item in Selection.objects)
        {
            EditorUtility.SetDirty(item);
        }
    }
}