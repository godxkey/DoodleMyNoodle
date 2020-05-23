using CCC.IO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SceneShortcutsGenerator
{
    const string GENERATED_FILE_NAME = "Assets/Code/Tools/SceneShortcuts.generated.cs";
    static readonly bool FLATTEN = true;

    static readonly string[] SCENE_SEARCH_FOLDERS =
    {
        "Assets/Scenes"
    };
    static readonly string[] SCENE_SEARCH_EXCLUDE =
    {
        "Assets/Scenes/Tests"
    };

    static readonly string[] MENU_ITEM_STRIPPED_WORDS =
    {
        "Scene Assets/"
    };

    [MenuItem("Tools/CodeGen/Generate Scene Shortcuts", priority = 10)]
    public static void Generate()
    {
        GenerateFileFrom(GetSceneAssets());
    }

    struct SceneElement
    {
        public string Path;
        public SceneAsset SceneAsset;
    }

    static List<SceneElement> GetSceneAssets()
    {
        string[] sceneAssetGuids = AssetDatabase.FindAssets("t:scene", SCENE_SEARCH_FOLDERS);

        List<SceneElement> sceneAssets = new List<SceneElement>();
        foreach (string guid in sceneAssetGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
            if (sceneAsset)
            {
                sceneAssets.Add(new SceneElement() { Path = path, SceneAsset = sceneAsset });
            }
        }

        return sceneAssets;
    }

    static void GenerateFileFrom(List<SceneElement> sceneAssets)
    {
        using (var fileWithWriter = FileX.OpenFileFlushedAndReadyToWrite(GENERATED_FILE_NAME))
        {
            StreamWriter writer = fileWithWriter.StreamWriter;

            writer.Write(
@"// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using UnityEditor;
using UnityEditor.SceneManagement;

public static class SceneShortcuts
{

");
            sceneAssets.Sort((a, b) => a.Path.CompareTo(b.Path));

            int prio = 0;
            string lastFolder = "";
            foreach (SceneElement scene in sceneAssets)
            {
                if (ShouldSceneBeExcluded(scene.Path))
                {
                    continue;
                }


                string sceneName = Path.GetFileNameWithoutExtension(scene.Path);
                string scenePathNoExt = PathX.RemoveExtension(scene.Path);
                string folder = scenePathNoExt.RemoveFirst(sceneName);
                string methodName = scenePathNoExt.Replace(' ', '_').Replace('/', '_');

                if (lastFolder != folder)
                    prio += 1000;

                string menuItemName;
                if (FLATTEN)
                {
                    menuItemName = sceneName;
                }
                else
                {
                    menuItemName = scenePathNoExt;
                    foreach (string searchFolder in SCENE_SEARCH_FOLDERS)
                    {
                        if (scenePathNoExt.StartsWith(searchFolder))
                        {
                            menuItemName = menuItemName.Remove(0, searchFolder.Length + 1);
                        }
                    }

                    foreach (var word in MENU_ITEM_STRIPPED_WORDS)
                    {
                        menuItemName = menuItemName.RemoveFirst(word);
                    }
                }

                writer.WriteLine($@"    [MenuItem(""Scene Shortcuts/{menuItemName}"", priority = {prio++})]");
                writer.WriteLine($@"    public static void {methodName}() => LoadScene(""{scene.Path}"");");
                writer.WriteLine();

                lastFolder = folder;
            }
            writer.Write(
@"
    private static void LoadScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}
");
            fileWithWriter.Dispose();
        }
    }

    private static bool ShouldSceneBeExcluded(string scenePath)
    {
        foreach (string excludeFolder in SCENE_SEARCH_EXCLUDE)
        {
            if (scenePath.Contains(excludeFolder))
            {
                return true;
            }
        }

        return false;
    }
}
