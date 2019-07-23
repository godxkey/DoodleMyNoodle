using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class HierarchySeparatorDrawer
{

    static GUIStyle style;
    
    //constructor
    static HierarchySeparatorDrawer()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        //get the gameObject reference using its instance ID
        GameObject go = (GameObject)EditorUtility.InstanceIDToObject(instanceID);

        if(style == null)
        {
            style = new GUIStyle(EditorStyles.toolbarButton);
            style.fixedHeight = 14;
        }

        if (go != null)
        {
            string name = go.name;
            if (name.Length >= 2
                && name[0] == '_'
                && name[name.Length - 1] == '_')
            {
                selectionRect.position += Vector2.up * 2;
                GUI.Box(selectionRect, name.Substring(1, name.Length - 2), style);
            }
        }
    }
}
