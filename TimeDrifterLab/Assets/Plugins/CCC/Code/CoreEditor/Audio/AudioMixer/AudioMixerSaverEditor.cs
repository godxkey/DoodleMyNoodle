using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System;

[CustomEditor(typeof(AudioMixerSaver))]
public class AudioMixerSaverEditor : Editor
{
    SerializedProperty fileName;
    SerializedProperty mixer;
    SerializedProperty loadOnEnable;
    GUIStyle runtimeStyle;
    AudioMixerSaver audioMixerSaves;
    AudioMixerSaver.ChannelType[] channelTypes;

    void CheckResources()
    {
        if (fileName == null)
            fileName = serializedObject.FindProperty("fileName");
        if (mixer == null)
            mixer = serializedObject.FindProperty("mixer");
        if (loadOnEnable == null)
            loadOnEnable = serializedObject.FindProperty("loadOnInit");

        if (runtimeStyle == null)
        {
            runtimeStyle = new GUIStyle(EditorStyles.boldLabel);
            runtimeStyle.normal.textColor = new Color(0.65f, 0f, 0f);
        }
        audioMixerSaves = (AudioMixerSaver)target;

        var values = Enum.GetValues(typeof(AudioMixerSaver.ChannelType));

        channelTypes = new AudioMixerSaver.ChannelType[values.Length];
        values.CopyTo(channelTypes, 0);
    }

    public override void OnInspectorGUI()
    {
        EditorHelper.DrawOpenFileLocation(Application.persistentDataPath);

        CheckResources();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(fileName, true);
        EditorGUILayout.PropertyField(mixer, true);


        EditorGUILayout.Space();
        if (GUILayout.Button("Revert to defaults"))
        {
            audioMixerSaves.SetDefaults();
        }
        if (GUILayout.Button("Save to disk"))
        {
            audioMixerSaves.Save();
        }
        if (GUILayout.Button("Load from disk"))
        {
            audioMixerSaves.Load();
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("DATA", runtimeStyle);

        for (int i = 0; i < channelTypes.Length; i++)
        {
            DrawChannel(channelTypes[i].ToString(), channelTypes[i]);
        }
    }

    private void DrawChannel(string label, ref AudioMixerSaver.ChannelData channel)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(label);
        channel.muted = EditorGUILayout.Toggle("Muted", channel.muted);
        channel.dbBoost = EditorGUILayout.FloatField("Db Boost", channel.dbBoost);
    }

    private void DrawChannel(string label, AudioMixerSaver.ChannelType channelType)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(label);


        EditorGUI.BeginChangeCheck();

        var muted = EditorGUILayout.Toggle("Muted", audioMixerSaves.GetMuted(channelType));
        var volume = EditorGUILayout.FloatField("Db Boost", audioMixerSaves.GetVolume(channelType));


        if (EditorGUI.EndChangeCheck())
        {
            audioMixerSaves.SetMuted(channelType, muted);
            audioMixerSaves.SetVolume(channelType, volume);
        }
    }
}
