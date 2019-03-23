using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine;

public static class SaveHelper
{
    static public void InstantSave(string path, object graph)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.OpenOrCreate);
        bf.Serialize(file, graph);
        file.Close();
    }

    static public object InstantLoad(string path)
    {
        if (!FileExists(path))
            return null;

        object obj = null;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);

        try
        {
            obj = bf.Deserialize(file);
        }
        catch (Exception e)
        {
            DebugService.LogError("Failed to deserialize the following file:\n" + path + "\n\nError:\n" + e.Message);
        }

        file.Close();

        return obj;
    }

    static public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    static public bool DeleteFile(string path)
    {
        if (!FileExists(path))
            return false;

        File.Delete(path);
        return true;
    }
}
