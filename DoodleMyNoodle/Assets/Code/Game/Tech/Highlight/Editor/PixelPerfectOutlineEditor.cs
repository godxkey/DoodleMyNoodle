#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// CODE TAKEN FROM PIXEL PERFECT OUTLINE PACKAGE

[CustomEditor(typeof(PixelPerfectOutline))]
public class PixelPerfectOutlineEditor : Editor
{
    Texture2D mytex;
    // Start is called before the first frame update
    private void OnEnable() {
        mytex = AssetDatabase.LoadAssetAtPath("Assets/PixelPerfectOutliner/Images/PixelPerfectBanner.png", typeof(Texture2D)) as Texture2D;
    }

    private void Awake() {
        mytex = AssetDatabase.LoadAssetAtPath("Assets/PixelPerfectOutliner/Images/PixelPerfectBanner.png", typeof(Texture2D)) as Texture2D;
    }

    public override void OnInspectorGUI() {

        PixelPerfectOutline obj = (PixelPerfectOutline)target;

        GUIStyle alphaStyle = new GUIStyle();
        alphaStyle.alignment = TextAnchor.MiddleRight;
        alphaStyle.fontSize = 12;

        GUIStyle horizontalLine;
        horizontalLine = new GUIStyle();
        horizontalLine.margin = new RectOffset(0, 0, 4, 4);
        horizontalLine.fixedHeight = 1;

        obj.alphaThreshold = EditorGUILayout.IntSlider("Alpha Threshold", obj.alphaThreshold, 1, 255);
        obj.thickness = EditorGUILayout.IntSlider("Thickness",obj.thickness, 1, 10);

        GUILayout.Space(10f);

        obj.includeChildren = EditorGUILayout.Toggle("Include Children",obj.includeChildren);
        obj.recursive = EditorGUILayout.Toggle("Recursive",obj.recursive);
        GUILayout.Space(10f);
        obj.compressFile = EditorGUILayout.Toggle("Compress", obj.compressFile);
        obj.save = EditorGUILayout.Toggle("Save Outline", obj.save);
        obj.includingCharacter = EditorGUILayout.Toggle("including source image", obj.includingCharacter);

       if( obj.includingCharacter) {
            obj.save = true;
        }
        
        obj.savePath = EditorGUILayout.TextField("Path: ", obj.savePath);
        GUILayout.Space(20f);
        obj.worldColor = (WorldColor)EditorGUILayout.EnumPopup("", obj.worldColor);
        GUILayout.Space(5f);
        obj.color = EditorGUILayout.ColorField("Outline Color:", obj.color);

        GUILayout.Space(10f);

        string[] options = new string[]{"Bilinear","Point","Trilinear" };
        obj.filterMode = EditorGUILayout.Popup("Filter Mode", obj.filterMode, options);

        if( obj.start) {
            GUILayout.Space(20f);
            using (var o = new EditorGUILayout.HorizontalScope()) {
                EditorGUI.ProgressBar(new Rect(o.rect.x,o.rect.y,o.rect.width,20f),obj.progress,"Creating");
            }
        }
        GUIStyle buttonStyle = new GUIStyle();
        buttonStyle.alignment = TextAnchor.LowerCenter;
        buttonStyle.fontSize = 18;
        GUI.backgroundColor = new Color(200,200,200,255);
        GUILayout.Space(20f);
        using (var h = new EditorGUILayout.HorizontalScope("Button")) {
            if (GUI.Button(new Rect(h.rect.x,h.rect.y,h.rect.width,40), GUIContent.none))
                obj.BeginOutline();
            GUILayout.Label("Create Outline", buttonStyle);
        }
        GUILayout.Space(10f);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(obj);
            EditorSceneManager.MarkSceneDirty(obj.gameObject.scene);
        }
    }
}

#endif