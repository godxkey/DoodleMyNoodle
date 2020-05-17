using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class DeleteEmptyFolders
{
    [MenuItem("Tools/Delete Empty Folders")]
    public static void Execute()
    {
        string deletedFolders = string.Empty;

        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
        foreach (var subDirectory in directoryInfo.GetDirectories("*.*", SearchOption.AllDirectories))
        {
            if (subDirectory.Exists && IsDirectoryEmpty(subDirectory))
            {
                subDirectory.Delete(true);

                string folderMetaFile = subDirectory.FullName + ".meta";
                if (File.Exists(folderMetaFile))
                {
                    File.Delete(folderMetaFile);
                }

                deletedFolders += subDirectory.FullName + '\n';
            }
        }

        Debug.Log("Deleted Folders:\n" + (deletedFolders.Length > 0 ? deletedFolders : "NONE"));

        AssetDatabase.Refresh();
    }

    private static bool IsDirectoryEmpty(DirectoryInfo subDirectory)
    {
        var filesInSubDirectory = subDirectory.GetFiles("*.*", SearchOption.AllDirectories);

        return filesInSubDirectory.Length == 0 || filesInSubDirectory.All(t => t.FullName.EndsWith(".meta"));
    }
}
