using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngineX;

/// <summary>
/// Used to permanently store notes
/// </summary>
public class EditorNotesDatabase : ScriptableObject
{
    [System.Serializable]
    struct SavedNote
    {
        public string Id;
        public string Text;
    }

    [SerializeField]
    List<SavedNote> _savedNotes = new List<SavedNote>();

    // We use the dictionary at runtime for quicker access
    Dictionary<string, string> _noteDictionary = new Dictionary<string, string>();

    void OnEnable()
    {
        Load();
    }

    void Load()
    {
        _noteDictionary.Clear();
        for (int i = 0; i < _savedNotes.Count; i++)
        {
            _noteDictionary.Add(_savedNotes[i].Id, _savedNotes[i].Text);
        }
    }

    public void Save()
    {
        _savedNotes.Clear();
        _savedNotes.Capacity = _noteDictionary.Count;
        foreach (var item in _noteDictionary)
        {
            _savedNotes.Add(new SavedNote() { Id = item.Key, Text = item.Value });
        }


        EditorUtility.SetDirty(this);
    }


    public bool ContainsNote(string id)
    {
        return _noteDictionary.ContainsKey(id);
    }

    public string GetNote(string id)
    {
        if(!_noteDictionary.TryGetValue(id, out string result))
        {
            result = null;
        }
        return result;
    }

    public void SetOrAddNote(string id, string text)
    {
        _noteDictionary.SetOrAdd(id, text);
    }

    public void DeleteNote(string id)
    {
        if (_noteDictionary.ContainsKey(id))
        {
            _noteDictionary.Remove(id);
        }
    }

    #region Asset Access
    private static EditorNotesDatabase s_cachedInstance;
    public static EditorNotesDatabase Instance
    {
        get
        {
            if (!s_cachedInstance)
            {
                s_cachedInstance = AssetDatabaseX.LoadOrCreateScriptableObjectAsset<EditorNotesDatabase>(EditorNotesSettings.ASSET_PATH_DATABASE);
            }

            return s_cachedInstance;
        }
    }
    #endregion
}
