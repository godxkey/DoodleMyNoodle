using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class HierarchySeparatorDrawer
{
    static GUIStyle s_styleLabel;
    static GUIStyle s_styleCount;

    //constructor
    static HierarchySeparatorDrawer()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        //get the gameObject reference using its instance ID
        GameObject go = (GameObject)EditorUtility.InstanceIDToObject(instanceID);

        if (s_styleLabel == null)
        {
            s_styleLabel = new GUIStyle(EditorStyles.objectField);
            s_styleLabel.fontSize = 12;
            s_styleLabel.alignment = TextAnchor.LowerLeft;
            s_styleLabel.fixedHeight = 16;

            s_styleCount = new GUIStyle(EditorStyles.label);
            s_styleCount.padding.bottom++;
            s_styleCount.normal.textColor = s_styleCount.normal.textColor.ChangedAlpha(0.4f);
            s_styleCount.hover.textColor = s_styleCount.hover.textColor.ChangedAlpha(0.4f);
            s_styleCount.focused.textColor = s_styleCount.focused.textColor.ChangedAlpha(0.4f);
            s_styleCount.active.textColor = s_styleCount.active.textColor.ChangedAlpha(0.5f);
        }

        if (go != null)
        {
            string name = go.name;
            if (name.Length >= 2
                && name[0] == '_'
                && name[name.Length - 1] == '_')
            {
                Color wasColor = GUI.color;
                
                GUI.color = GetBoxColor();
                
                GUIContent label = new GUIContent();
                label.text = name.Substring(1, name.Length - 2);
                GUI.Box(selectionRect, label, s_styleLabel);

                selectionRect.xMin += s_styleLabel.CalcSize(label).x - 18;
                GUI.Label(selectionRect, $"(x{go.transform.childCount})", s_styleCount);
                
                GUI.color = wasColor;
            }
        }
    }

    private static Color GetBoxColor()
    {
        Color c = GUI.color * 0.7f;
        c.a = 1;
        return c;
    }
}
