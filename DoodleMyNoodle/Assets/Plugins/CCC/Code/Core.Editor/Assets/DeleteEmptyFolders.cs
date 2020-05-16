using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

//public static class DeleteEmptyFolders
//{
//    [MenuItem("Tools/Delete Empty Folders")]
//    public static void Execute()
//    {
//        string deletedFolders = string.Empty;

//        DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
//        foreach (var subDirectory in directoryInfo.GetDirectories("*.*", SearchOption.AllDirectories))
//        {
//            if (subDirectory.Exists)
//            {
//                deletedFolders += ScanDirectory(subDirectory);
//            }
//        }

//        Debug.Log("Deleted Folders:\n" + (deletedFolders.Length > 0 ? deletedFolders : "NONE"));
//    }

//    private static string ScanDirectory(DirectoryInfo subDirectory)
//    {
//        string deletedFolders = string.Empty;

//        var filesInSubDirectory = subDirectory.GetFiles("*.*", SearchOption.AllDirectories);

//        if (filesInSubDirectory.Length == 0 || !filesInSubDirectory.Any(t => t.FullName.EndsWith(".meta") == false))
//        {
//            subDirectory.Delete(true);
//            deletedFolders += subDirectory.FullName + "\n";
//        }
//        else
//        {

//        }

//        return deletedFolders;
//    }
//}
