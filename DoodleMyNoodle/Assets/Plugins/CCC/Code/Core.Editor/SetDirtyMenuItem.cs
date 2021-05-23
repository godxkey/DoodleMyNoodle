using UnityEngine;
using UnityEditor;

public static class SetDirtyMenuItem
{
    [MenuItem("Assets/Set Dirty")]
    public static void SetDirty()
    {
        foreach (Object item in Selection.GetFiltered<Object>(SelectionMode.DeepAssets | SelectionMode.Editable))
        {
            EditorUtility.SetDirty(item);
        }
    }
}