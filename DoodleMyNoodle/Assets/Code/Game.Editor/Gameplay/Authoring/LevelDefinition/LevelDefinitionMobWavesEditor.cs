using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelDefinitionMobWaves))]
public class LevelDefinitionMobWavesEditor : Editor
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
        if (!Application.isPlaying && LevelDefinitionMobWavesPreviewer.Instance == null)
        {
            LevelDefinitionMobWavesPreviewer.CreateInstance();
            LevelDefinitionMobWavesPreviewer.Instance.DisplayLevel(target as LevelDefinitionMobWaves);
        }
    }

    private void OnDisable()
    {
        if (LevelDefinitionMobWavesPreviewer.Instance != null)
            LevelDefinitionMobWavesPreviewer.DestroyInstance();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
        {
            if (LevelDefinitionMobWavesPreviewer.Instance == null)
                LevelDefinitionMobWavesPreviewer.CreateInstance();
            LevelDefinitionMobWavesPreviewer.Instance.DisplayLevel(target as LevelDefinitionMobWaves);
        }
    }
}
