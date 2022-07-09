using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelDefinitionAuthMobWaves))]
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
        if (!Application.isPlaying && LevelDefinitionAuthMobWavesPreviewer.Instance == null)
        {
            LevelDefinitionAuthMobWavesPreviewer.CreateInstance();
            LevelDefinitionAuthMobWavesPreviewer.Instance.DisplayLevel(target as LevelDefinitionAuthMobWaves);
        }
    }

    private void OnDisable()
    {
        if (LevelDefinitionAuthMobWavesPreviewer.Instance != null)
            LevelDefinitionAuthMobWavesPreviewer.DestroyInstance();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
        {
            if (LevelDefinitionAuthMobWavesPreviewer.Instance == null)
                LevelDefinitionAuthMobWavesPreviewer.CreateInstance();
            LevelDefinitionAuthMobWavesPreviewer.Instance.DisplayLevel(target as LevelDefinitionAuthMobWaves);
        }
    }
}
