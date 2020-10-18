using System;
using UnityEditor;
using UnityEngine;
using UnityEngineX;

#if UNITY_EDITOR
public class ViewEditorGhost : MonoBehaviour
{
    public GameObject BindedSimGameObject;
}

[InitializeOnLoad]
public static class ViewEditorGhostEditorBehaviour
{
    //private const string GHOST_ICON_GUID = "655343e5fe169bd4bbf84e75702d6aed";

    //private static bool s_init = false;
    //private static Texture s_ghostIcon;
    private static GameObject s_redirectSelectionTo;

    static ViewEditorGhostEditorBehaviour()
    {
        //EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
        Selection.selectionChanged += OnSelectionChanged;
        EditorApplication.update += OnUpdate;
    }

    private static void OnUpdate()
    {
        if (s_redirectSelectionTo != null)
            Selection.activeGameObject = s_redirectSelectionTo;
        s_redirectSelectionTo = null;
    }

    private static void OnSelectionChanged()
    {
        GameObject[] selections = Selection.gameObjects;
        if (selections != null && selections.Length == 1)
        {
            Transform tr = selections[0].transform;
            while (tr)
            {
                if (tr.TryGetComponent(out ViewEditorGhost ghost))
                {
                    s_redirectSelectionTo = ghost.BindedSimGameObject;
                    //EditorApplication.QueuePlayerLoopUpdate();
                    return;
                }

                tr = tr.parent;
            }
        }
    }

    //private static void OnHierarchyItemGUI(int instanceID, Rect selectionRect)
    //{
    //    if (s_redirectSelectionTo != null)
    //        Selection.activeGameObject = s_redirectSelectionTo;
    //    s_redirectSelectionTo = null;

    //    if (Event.current.type != EventType.Repaint && Event.current.type != EventType.Layout)
    //        return;

    //    InitializeIfNeeded();

    //    GameObject gameObject = (GameObject)EditorUtility.InstanceIDToObject(instanceID);
    //    if (gameObject != null && gameObject.HasComponent<ViewEditorGhost>())
    //    {
    //        Rect rect = new Rect(selectionRect);
    //        rect.width = 15;
    //        rect.height = 15;
    //        GUI.DrawTexture(rect, s_ghostIcon);
    //    }
    //}

    //private static void InitializeIfNeeded()
    //{
    //    if (s_init)
    //        return;

    //    s_init = true;

    //    s_ghostIcon = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(GHOST_ICON_GUID));
    //}
}
#endif