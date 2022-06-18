using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(SceneInfo))]
public class SceneInfoEditor : Editor
{
    private SceneInfo TargetSceneInfo => target as SceneInfo;

    private enum BuildState
    {
        NotInBuild,
        DisabledInBuild,
        EnabledInBuild
    }

    private static void AddToBuild(string assetPath)
    {
        var original = EditorBuildSettings.scenes;
        var newSettings = new EditorBuildSettingsScene[original.Length + 1];
        Array.Copy(original, newSettings, original.Length);

        EditorBuildSettingsScene sceneToAdd = new EditorBuildSettingsScene(assetPath, true);
        newSettings[newSettings.Length - 1] = sceneToAdd;

        EditorBuildSettings.scenes = newSettings;
    }
    private static void EnableInBuild(int buildIndex)
    {
        var scenes = EditorBuildSettings.scenes;
        scenes[buildIndex].enabled = true;
        EditorBuildSettings.scenes = scenes;
    }
    private static void DisableInBuild(int buildIndex)
    {
        var scenes = EditorBuildSettings.scenes;
        scenes[buildIndex].enabled = false;
        EditorBuildSettings.scenes = scenes;
    }
    private static void PutFirstInBuild(int buildIndex)
    {
        var original = EditorBuildSettings.scenes;
        var newSettings = new EditorBuildSettingsScene[original.Length];

        EditorBuildSettingsScene theOne = original[buildIndex];

        int u = 1;
        for (int i = 0; i < original.Length; i++)
        {
            if (i != buildIndex)
            {
                newSettings[u] = original[i];
                u++;
            }
        }
        newSettings[0] = theOne;

        EditorBuildSettings.scenes = newSettings;
    }
    private static void RemoveFromBuild(int buildIndex)
    {
        var original = EditorBuildSettings.scenes;
        var newSettings = new EditorBuildSettingsScene[original.Length - 1];

        int u = 0;
        for (int i = 0; i < original.Length; i++)
        {
            if (i != buildIndex)
            {
                newSettings[u] = original[i];
                u++;
            }
        }

        EditorBuildSettings.scenes = newSettings;
    }

    /// <summary>
    ///  -1 -> not in build   0 -> disabled in build   2 -> enabled in build
    /// </summary>
    private BuildState GetBuildState(out string assetPath, out int indexInBuild)
    {
        SceneAsset sceneAsset = TargetSceneInfo.Editor_GetScene();
        assetPath = AssetDatabase.GetAssetOrScenePath(sceneAsset);
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].path == assetPath)
            {
                indexInBuild = i;
                if (scenes[i].enabled)
                    return BuildState.EnabledInBuild;
                else
                    return BuildState.DisabledInBuild;
            }
        }

        indexInBuild = -1;
        return BuildState.NotInBuild;
    }

    public override void OnInspectorGUI()
    {
        Color guiColor = GUI.color;
        SceneAsset sceneAsset = TargetSceneInfo.Editor_GetScene();

        if (sceneAsset != null)
        {
            string assetPath;
            int indexInBuild;

            // -1 -> not in build   0 -> disabled in build   2 -> enabled in build
            BuildState buildState = GetBuildState(out assetPath, out indexInBuild);

            if (buildState ==  BuildState.NotInBuild)
            {
                GUI.color = new Color(0.6f, 1, 0.6f);
                if (GUILayout.Button("ADD TO BUILD SETTINGS"))
                {
                    AddToBuild(assetPath);
                }
                GUI.color = guiColor;
            }
            else
            {
                GUI.color = new Color(1, 0.6f, 0.6f);
                if (GUILayout.Button("REMOVE FROM BUILD SETTINGS"))
                {
                    RemoveFromBuild(indexInBuild);
                }

                GUI.color = guiColor;
                EditorGUILayout.BeginHorizontal();
                if (EditorGUILayout.Toggle("Included in build", buildState ==  BuildState.EnabledInBuild) != (buildState == BuildState.EnabledInBuild))
                {
                    if (buildState ==  BuildState.EnabledInBuild)
                        DisableInBuild(indexInBuild);
                    else
                        EnableInBuild(indexInBuild);
                }

                bool isInFirstPlace = indexInBuild == 0;
                if (isInFirstPlace)
                    GUI.enabled = false;
                if (GUILayout.Button(isInFirstPlace ? "Is in first place" : "Put in first place"))
                {
                    PutFirstInBuild(indexInBuild);
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();
            }
        }

        if (sceneAsset != null && sceneAsset.name != TargetSceneInfo.SceneName)
        {
            GUI.color = new Color(0.6f, 1, 0.6f);
        }
        else
        {
            GUI.enabled = false;
        }

        if (GUILayout.Button("Refresh Name"))
        {
            TargetSceneInfo.Editor_RefreshSceneName();
            EditorUtility.SetDirty(TargetSceneInfo);
        }

        GUI.enabled = true;
        GUI.color = guiColor;

        base.OnInspectorGUI();
    }
}
#endif
