using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelDefinition))]
public class LevelDefinitionEditor : Editor
{
    protected override void OnHeaderGUI()
    {
        base.OnHeaderGUI();

        if (targets != null && targets.Length == 1)
        {
            if (GUILayout.Button("Preview In Scene"))
            {
            }
            GUILayout.Space(10);
        }
    }

    private void OnEnable()
    {
        if (!Application.isPlaying && LevelDefinitionPreviewer.Instance == null)
        {
            LevelDefinitionPreviewer.CreateInstance();
            LevelDefinitionPreviewer.Instance.DisplayLevel(target as LevelDefinition);
        }
    }

    private void OnDisable()
    {
        if (LevelDefinitionPreviewer.Instance != null)
            LevelDefinitionPreviewer.DestroyInstance();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
        {
            if (LevelDefinitionPreviewer.Instance == null)
                LevelDefinitionPreviewer.CreateInstance();
            LevelDefinitionPreviewer.Instance.DisplayLevel(target as LevelDefinition);
        }
    }
}
