using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CreateBackupCameraShortcut
{

    [MenuItem("GameObject/Backup Camera", false, 10)]
    static void CreateCustomGameObject(MenuCommand menuCommand)
    {
        string[] guids = AssetDatabase.FindAssets("\"BackupCamera\" t:prefab");

        if (guids.Length == 0)
        {
            Debug.LogWarning("We did not find a prefab called BackupCamera");
            return;
        }

        if (guids.Length > 1)
        {
            Debug.LogWarning("We found more than one prefab called BackupCamera");
        }

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[0]));
        GameObject instance =  (GameObject)PrefabUtility.InstantiatePrefab(prefab, EditorSceneManager.GetActiveScene());

        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(instance, "Create " + instance.name);
        Selection.activeObject = instance;
    }
}
