using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(GameSavesManager))]
public class GameSavesManagerEditor : Editor
{
    string[] typeNames;
    Array values;
    GameSavesManager saveManager;

    void OnEnable()
    {
        saveManager = target as GameSavesManager;
        typeNames = Enum.GetNames(typeof(GameSaveCategory));

        if (!saveManager.VerifyArrayIntegrity())
        {
            if (AssetDatabase.Contains(saveManager))
                EditorUtility.SetDirty(saveManager);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        for (int i = 0; i < typeNames.Length; i++)
        {
            DrawDataSaver(i);
        }
    }

    private void DrawDataSaver(int index)
    {
        GameSaveCategory type = (GameSaveCategory)index;
        var saver = saveManager.GetDataSaver(type);
        var newSaver = EditorGUILayout.ObjectField(typeNames[index], saver, typeof(DataSaver), false) as DataSaver;
        if (newSaver != saver)
        {
            saveManager.SetDataSaver(type, newSaver);
            if(saveManager.gameObject.scene.name == null)
            {
                EditorUtility.SetDirty(saveManager);
            }
            else
            {
                EditorSceneManager.MarkSceneDirty(saveManager.gameObject.scene);
            }
        }
    }
}
