using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class FileScriptableInterface : ScriptableObject
{
    [Suffix(FILE_EXTENSION), SerializeField] protected string fileName = "someData";
    public const string FILE_EXTENSION = ".dat";

    /// <summary>
    /// Évenement appelé lorsqu'on réassigne les données (load/clear)
    /// </summary>
    public event Action OnReassignData;

    /// <summary>
    /// Read/Write operation queue. C'est une queue qui assure l'ordonnancement des opérations read/write
    /// </summary>
    private Queue<Action> rwoQueue = new Queue<Action>();

    private bool lateSave = false;
    private Queue<Action> pendingLateSaveCallbacks = new Queue<Action>();

    private bool lateLoad = false;
    private Queue<Action> pendingLateLoadCallbacks = new Queue<Action>();

    public string CompletePath
    {
        get { return Application.persistentDataPath + "/" + fileName + FILE_EXTENSION; }
    }

    /// <summary>
    /// Retourne vrai si l'objet a Load au moins une fois à date
    /// </summary>
    public bool HasEverLoaded { get; private set; }


    private void _OverwriteLocalData(object graph)
    {
        OverwriteLocalData(graph);
        OnReassignData?.Invoke();
    }
    protected void _SetDefaultLocalData()
    {
        OnReassignData?.Invoke();
        SetDefaultLocalData();
    }
    protected abstract void OverwriteLocalData(object graph);
    protected abstract void SetDefaultLocalData();
    protected abstract object GetLocalData();

    private void OnEnable()
    {
        lateSave = false;
        HasEverLoaded = false;
        OnReassignData = null;
        rwoQueue.Clear();
        pendingLateSaveCallbacks.Clear();
        pendingLateLoadCallbacks.Clear();
    }

    /// <summary>
    /// Semblable au SaveAsync, mais on va laisser 1 frame s'écouler avant.
    /// <para/>
    /// Si LateSave est appelé plusieur fois, l'objet ne sera que sauvegarder 1 fois.
    /// Ça nous permet d'appeler plusieur fois LateSave par frame sans avoir à se soucier
    /// du coût de performance de plusieurs sauvegardes.
    /// </summary>
    public void LateSave() { LateSave(null); }
    /// <summary>
    /// Semblable au SaveAsync, mais on va laisser 1 frame s'écouler avant.
    /// <para/>
    /// Si LateSave est appelé plusieur fois, l'objet ne sera que sauvegarder 1 fois.
    /// Ça nous permet d'appeler plusieur fois LateSave par frame sans avoir à se soucier
    /// du coût de performance de plusieurs sauvegardes.
    /// </summary>
    public void LateSave(Action callback)
    {
        //Add callback to queue
        if (callback != null)
            pendingLateSaveCallbacks.Enqueue(callback);

        if (lateSave)
            return;
        lateSave = true;

        CoroutineLauncherService.Instance.CallNextFrame(() =>
        {
            SaveAsync(() =>
            {
                //Clear all pending callbacks
                while (pendingLateSaveCallbacks.Count > 0)
                {
                    pendingLateSaveCallbacks.Dequeue().Invoke();
                }
                lateSave = false;
            });
        });
    }

    /// <summary>
    /// Semblable au LoadAsync, mais on va laisser 1 frame s'écouler avant.
    /// <para/>
    /// Si LateLoad est appelé plusieur fois, l'objet ne sera que loadé 1 fois.
    /// Ça nous permet d'appeler plusieur fois LateLoad par frame sans avoir à se soucier
    /// du coût de performance de plusieurs loading.
    /// </summary>
    public void LateLoad() { LateLoad(null); }
    /// <summary>
    /// Semblable au LoadAsync, mais on va laisser 1 frame s'écouler avant.
    /// <para/>
    /// Si LateLoad est appelé plusieur fois, l'objet ne sera que loadé 1 fois.
    /// Ça nous permet d'appeler plusieur fois LateLoad par frame sans avoir à se soucier
    /// du coût de performance de plusieurs loading.
    /// </summary>
    public void LateLoad(Action callback)
    {
        //Add callback to queue
        if (callback != null)
            pendingLateLoadCallbacks.Enqueue(callback);

        if (lateLoad)
            return;
        lateLoad = true;

        CoroutineLauncherService.Instance.CallNextFrame(() =>
        {
            LoadAsync(() =>
            {
                //Clear all pending callbacks
                while (pendingLateLoadCallbacks.Count > 0)
                {
                    pendingLateLoadCallbacks.Dequeue().Invoke();
                }
                lateLoad = false;
            });
        });
    }

    #region Save/Load
    public void LoadAsync() { LoadAsync(null); }
    public void LoadAsync(Action onLoadComplete)
    {
        AddRWOperation(() =>
        {
            var path = CompletePath;

            //Exists ?
            if (SaveHelper.FileExists(path))
            {
                SaveService.Instance.ThreadLoad(path,
                    delegate (object graph)
                    {
                        _OverwriteLocalData(graph);
                        HasEverLoaded = true;

                        onLoadComplete?.Invoke();

                        CompleteRWOperation();
                    });
            }
            else
            {
                //Nouveau fichier !
                _SetDefaultLocalData();
                SaveAsync(onLoadComplete);

                CompleteRWOperation();
            }
        });
    }

    public void Load() { Load(null); }
    public void Load(Action onLoadComplete)
    {
        AddRWOperation(() =>
        {
            string path = CompletePath;

            //Exists ?
            if (SaveHelper.FileExists(path))
            {
                //Load and apply
                object graph = SaveHelper.InstantLoad(path);
                _OverwriteLocalData(graph);
                HasEverLoaded = true;

                onLoadComplete?.Invoke();
            }
            else
            {
                //Nouveau fichier !
                _SetDefaultLocalData();

                Save(onLoadComplete);
            }

            CompleteRWOperation();
        });
    }

    public void SaveAsync() { SaveAsync(null); }
    public void SaveAsync(Action onSaveComplete)
    {
        AddRWOperation(() =>
        {
            Action onComplete = () =>
            {
                onSaveComplete?.Invoke();

                CompleteRWOperation();
            };

            object data = GetLocalData();
            if (data != null)
                SaveService.Instance.ThreadSave(CompletePath, data, onComplete);
            else
                onComplete();
        });
    }

    public void Save() { Save(null); }
    public void Save(Action onSaveComplete)
    {
        AddRWOperation(() =>
        {
            object data = GetLocalData();
            if (data != null)
                SaveHelper.InstantSave(CompletePath, data);

            onSaveComplete?.Invoke();

            CompleteRWOperation();
        });
    }

    public void ClearSave() { ClearSave(null); }
    public void ClearSave(Action onComplete)
    {
        AddRWOperation(() =>
        {
            SaveHelper.DeleteFile(CompletePath);
            _SetDefaultLocalData();

            onComplete?.Invoke();

            CompleteRWOperation();
        });
    }
    #endregion

    #region RW Operations
    private void AddRWOperation(Action action)
    {
        //On s'enfile
        rwoQueue.Enqueue(action);

        //Sommes nous les premier a etre dans la queue ?
        if (rwoQueue.Count == 1)
            action();
    }

    private void CompleteRWOperation()
    {
        //On enleve la derniere action
        rwoQueue.Dequeue();

        //On execute la prochaine s'il y en a une
        if (rwoQueue.Count > 0)
            rwoQueue.Peek()();
    }
    #endregion
}
