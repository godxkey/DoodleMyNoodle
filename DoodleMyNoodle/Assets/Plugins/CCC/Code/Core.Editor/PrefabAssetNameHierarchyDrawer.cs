using System;
using UnityEditor;
using UnityEngine;

namespace UnityEditorX
{
    [InitializeOnLoad]
    public class PrefabAssetNameHierarchyDrawer
    {
        private static GUIStyle s_style;
        private static bool s_init = false;

        static PrefabAssetNameHierarchyDrawer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
        }

        private static void OnHierarchyItemGUI(int instanceID, Rect selectionRect)
        {
            InitializeIfNeeded();

            GameObject gameObject = (GameObject)EditorUtility.InstanceIDToObject(instanceID);
            if (gameObject != null && PrefabUtility.IsAnyPrefabInstanceRoot(gameObject))
            {
                var originalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject));
                if (originalPrefab != null)
                {
                    string cleanedGOName = StripGameObjectCloneNumberFromName(gameObject.name); // remove (1) from "PFB_MyElement(1)"

                    if (cleanedGOName != originalPrefab.name)
                    {
                        var goLabel = EditorGUIUtilityX.TempContent(gameObject.name);
                        Rect goLabelRect = GUILayoutUtility.GetRect(goLabel, EditorStyles.label, GUILayout.ExpandWidth(false));
                        selectionRect.position += Vector2.right * (goLabelRect.width + 20);
                        GUI.Label(selectionRect, $"({originalPrefab.name})", s_style);
                    }
                }
            }
        }

        private static void InitializeIfNeeded()
        {
            if (s_init)
                return;

            s_init = true;

            s_style = new GUIStyle(GUI.skin.GetStyle("PR PrefabLabel")); // Unity's builtin style for prefab name display
            var c = s_style.normal.textColor;
            c.a *= 0.65f;
            s_style.normal.textColor = c;
            s_style.fontSize = 10;
            s_style.alignment = TextAnchor.LowerLeft;
            s_style.fixedHeight = 14;
            s_style.fontStyle = FontStyle.Italic;
        }

        private static string StripGameObjectCloneNumberFromName(string name)
        {
            if (!name.EndsWith(")"))
            {
                return name;
            }

            int openParentheseIndex = name.LastIndexOf('(');

            if (openParentheseIndex < 1) // open parenthese must exist beyond first character
                return name;

            for (int i = openParentheseIndex + 1; i < name.Length - 1; i++)
            {
                if (!char.IsDigit(name[i]))
                    return name;
            }

            return name.Substring(0, openParentheseIndex - 1);
        }
    }
}
