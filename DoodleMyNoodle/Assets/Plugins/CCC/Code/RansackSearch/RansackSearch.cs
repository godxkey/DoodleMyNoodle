using System.Linq;
using UnityEditor;
using UnityEngineX;

public static partial class RansackSearch
{
    private const string MENU_NAME = "Assets/Find References (Ransack)/";


    [MenuItem(itemName: MENU_NAME + "Prefabs, Scriptable Objects, Scenes", priority = 20)]
    public static void FindReferencesInPrefabs()
    {
        RunRansack(Selection.assetGUIDs[0], "*.prefab| *.asset| *.unity");
    }

    [MenuItem(itemName: MENU_NAME + "Prefabs, Scriptable Objects", priority = 21)]
    public static void FindReferencesInPrefabsAndScriptableObjects()
    {
        RunRansack(Selection.assetGUIDs[0], "*.prefab| *.asset");
    }

    [MenuItem(itemName: MENU_NAME + "All", priority = 22)]
    public static void FindReferencesInAll()
    {
        RunRansack(Selection.assetGUIDs[0], "");
    }

    [MenuItem(itemName: MENU_NAME + "Open Agent Ransack", priority = 23)]
    public static void OpenRansack()
    {
        RunRansack(Selection.assetGUIDs[0], "*.prefab| *.asset| *.unity", insideEditor: false);
    }

    [MenuItem(itemName: MENU_NAME + "Prefabs, Scriptable Objects, Scenes", validate = true, priority = 20)]
    [MenuItem(itemName: MENU_NAME + "Prefabs, Scriptable Objects", validate = true, priority = 21)]
    [MenuItem(itemName: MENU_NAME + "All", validate = true, priority = 22)]
    [MenuItem(itemName: MENU_NAME + "Open Agent Ransack", validate = true, priority = 23)]
    private static bool ValidateMenu() => Selection.assetGUIDs.Length == 1;

    private static void RunRansack(string search, string fileNames, bool insideEditor = true)
    {
        if (insideEditor)
        {
            RansackQuery ransackQuery = new RansackQuery(search, fileNames);
            ProcessWindow.Show(ransackQuery);
        }
        else
        {
            RansackQuery.OpenRansackExternal(search, fileNames);
        }
    }
}
