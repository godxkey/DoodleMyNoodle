using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using System;
using UnityEngine;

public class SaveService : MonoCoreService<SaveService>
{
    public override void Initialize(Action<ICoreService> onComplete) => onComplete(this);

    public void ThreadSave(string path, object graph, Action onComplete = null)
    {
        if (Application.isPlaying)
        {
            Thread t = new Thread(new ThreadStart(() => ThreadSave_Method(path, graph, onComplete)));
            t.Start();
        }
        else
        {
            ErrorLogThreadMethodInNonPlaying();
        }
    }
    void ThreadSave_Method(string path, object graph, Action onComplete = null)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.OpenOrCreate);
        bf.Serialize(file, graph);
        file.Close();

        MainThreadService.AddMainThreadCallbackFromThread(onComplete);
    }

    public void ThreadLoad(string path, Action<object> onComplete)
    {
        if (Application.isPlaying)
        {
            Thread t = new Thread(new ThreadStart(delegate () { ThreadLoad_Method(path, onComplete); }));
            t.Start();
        }
        else
        {
            ErrorLogThreadMethodInNonPlaying();
        }
    }
    void ThreadLoad_Method(string path, Action<object> onComplete)
    {
        if (!FileExists(path))
        {
            onComplete.Invoke(null);
            return;
        }

        object obj = null;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open);
        try
        {
            obj = bf.Deserialize(file);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to deserialize the following file:\n" + path + "\n\nError:\n" + e.Message);
        }
        file.Close();


        if (onComplete != null)
            MainThreadService.AddMainThreadCallbackFromThread(delegate ()
            {
                onComplete(obj);
            });
    }

    static private void ErrorLogThreadMethodInNonPlaying() { Debug.LogError("Cannot use " + nameof(SaveService) + "'s threaded methods when the game is not running"); }

    public void InstantSave(string path, object graph)
    {
        SaveHelper.InstantSave(path, graph);
    }

    public object InstantLoad(string path)
    {
        return   SaveHelper.InstantLoad(path);
    }

    public bool FileExists(string path)
    {
        return SaveHelper.FileExists(path);
    }

    public bool DeleteFile(string path)
    {
        return SaveHelper.DeleteFile(path);
    }
}
