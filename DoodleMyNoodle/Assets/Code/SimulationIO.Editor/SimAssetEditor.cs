using UnityEditor;
using UnityEditorX;
using UnityEngine;
using UnityEngineX;

[CustomEditor(typeof(SimAsset))]
public class SimAssetEditor : Editor
{
    private SerializedProperty _guidProp;
    private SerializedProperty _bindedViewPrefabProp;
    private SerializedProperty _bindedViewTileProp;
    private SerializedProperty _viewTechTypeProp;
    private SerializedProperty _showGhostProp;
    private SerializedProperty _hasTransform;

    protected void OnEnable()
    {
        _guidProp = serializedObject.FindProperty("_guid");
        _bindedViewPrefabProp = serializedObject.FindProperty("_bindedViewPrefab");
        _bindedViewTileProp = serializedObject.FindProperty("_bindedViewTile");
        _viewTechTypeProp = serializedObject.FindProperty("_viewTechType");
        _showGhostProp = serializedObject.FindProperty("_showGhost");
        _hasTransform = serializedObject.FindProperty("_hasTransform");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var simAsset = (SimAsset)target;
        bool wasGUI = GUI.enabled;

        EditorGUILayout.PropertyField(_hasTransform);

        if (_hasTransform.boolValue)
        {
            if (string.IsNullOrEmpty(_guidProp.stringValue))
            {
                EditorGUILayout.HelpBox("Only prefabs can bind a view", MessageType.Info);
                GUI.enabled = false;
            }
            else if (PrefabUtility.IsPartOfNonAssetPrefabInstance(simAsset))
            {
                // disable properties so we cannot add overrides in scenes
                EditorGUILayout.HelpBox("Open prefab to edit", MessageType.Info);
                GUI.enabled = false;
            }

            EditorGUILayout.PropertyField(_viewTechTypeProp);

            var viewTechType = (ViewTechType)_viewTechTypeProp.enumValueIndex;
            if (viewTechType == ViewTechType.GameObject)
            {
                EditorGUILayout.PropertyField(_bindedViewPrefabProp, EditorGUIUtilityX.TempContent("View Prefab"));
            }
            else if (viewTechType == ViewTechType.Tile)
            {
                EditorGUILayout.PropertyField(_bindedViewTileProp, EditorGUIUtilityX.TempContent("View Tile"));
            }

            GUI.enabled = wasGUI;

            if (viewTechType == ViewTechType.GameObject && _bindedViewPrefabProp.objectReferenceValue != null)
            {
                EditorGUILayout.PropertyField(_showGhostProp);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}