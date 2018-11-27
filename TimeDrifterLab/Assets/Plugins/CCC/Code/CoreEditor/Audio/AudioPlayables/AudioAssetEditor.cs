using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioPlayable), true)]
public class AudioPlayableEditor : Editor
{
    [SerializeField] private AudioSource previewAudioSource;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AudioPlayable playable = target as AudioPlayable;

        EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

        GUILayoutOption largeHeight = GUILayout.Height(18 * 2 + 3);

        GUILayout.BeginHorizontal(largeHeight);

        GUI.enabled = previewAudioSource.isPlaying;
        if (GUILayout.Button("Stop", largeHeight))
        {
            previewAudioSource.Stop();
        }
        GUI.enabled = true;

        GUILayout.BeginVertical();
        if (GUILayout.Button("Preview"))
        {
            playable.PlayOn(previewAudioSource);
        }
        if (GUILayout.Button("Preview looped"))
        {
            if (previewAudioSource.isPlaying)
                previewAudioSource.Stop();
            playable.PlayLoopedOn(previewAudioSource);
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();
    }

    public void OnEnable()
    {
        previewAudioSource = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
    }

    public void OnDisable()
    {
        DestroyImmediate(previewAudioSource.gameObject);
    }

    public override bool RequiresConstantRepaint()
    {
        return true;
    }
}