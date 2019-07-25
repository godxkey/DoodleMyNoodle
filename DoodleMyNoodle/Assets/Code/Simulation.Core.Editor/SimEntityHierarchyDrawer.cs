using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class SimEntityHierarchyDrawer
{

    static GUIStyle style;

    //constructor
    static SimEntityHierarchyDrawer()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        //get the gameObject reference using its instance ID
        GameObject go = (GameObject)EditorUtility.InstanceIDToObject(instanceID);

        if (style == null)
        {
            style = new GUIStyle(EditorStyles.toolbarButton);
            style.fixedHeight = 14;
        }

        if (go != null)
        {
            Rect rect = new Rect(selectionRect.xMax - 45, selectionRect.yMin + 1, 45, selectionRect.height);

            SimEntity simEntity = go.GetComponent<SimEntity>();
            if (simEntity)
            {
                Color pastGUIColor = GUI.color;

                if (SimModules.isInitialized == simEntity.isPartOfSimulation)
                {
                    GUI.color = new Color(0.65f, 1, 0.65f);
                    GUI.Box(rect, "Sim", style);
                }
                else
                {
                    GUI.color = new Color(1, 0.65f, 0.65f);
                    GUI.Box(rect, "Sim", style);
                }

                GUI.color = pastGUIColor;
            }
        }
    }
}
