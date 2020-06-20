using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngineX;

public class ScriptDefineSymbolManagerSettings : ScriptableObject
{
    [System.Serializable]
    private class Profile : ScriptDefineSymbolManager.IProfile
    {
        public string Name;
        public List<string> Symbols = new List<string>();

        string ScriptDefineSymbolManager.IProfile.Name => Name;
        ReadOnlyList<string> ScriptDefineSymbolManager.IProfile.DefinedSymbols => Symbols.AsReadOnlyNoAlloc();
    }

    [SerializeField] private List<Profile> _profiles = new List<Profile>();
    [SerializeField] private List<string> _symbols = new List<string>();

    public ReadOnlyListDynamic<ScriptDefineSymbolManager.IProfile> Profiles => _profiles.AsReadOnlyNoAlloc().DynamicCast<ScriptDefineSymbolManager.IProfile>();
    public ReadOnlyList<string> Symbols => _symbols.AsReadOnlyNoAlloc();

    public void CreateSymbol(string symbol)
    {
        _symbols.AddUnique(symbol);
    }

    public void DeleteSymbol(string symbol)
    {
        foreach (var p in _profiles)
        {
            RemoveSymbolFromProfile(symbol, p);
        }
        _symbols.Remove(symbol);
    }

    public void AddSymbolInProfile(string symbol, ScriptDefineSymbolManager.IProfile profile)
    {
        if (_symbols.Contains(symbol))
        {
            if (profile is Profile p)
            {
                p.Symbols.AddUnique(symbol);
            }
        }
        else
        {
            Debug.LogError($"Symbol {symbol} doesn't exist.");
        }
    }

    public bool RemoveSymbolFromProfile(string symbol, ScriptDefineSymbolManager.IProfile profile)
    {
        if (profile is Profile p)
        {
            return p.Symbols.Remove(symbol);
        }

        return false;
    }

    public ScriptDefineSymbolManager.IProfile CreateProfile(string name)
    {
        if (!IsProfileNameValid(name))
        {
            return null;
        }

        var newProfile = new Profile();
        newProfile.Name = name;
        _profiles.Add(newProfile);
        _profiles.Sort((a, b) => a.Name.CompareTo(b.Name));
        return newProfile;
    }

    private bool IsProfileNameValid(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError($"Invalid profile name.");
            return false;
        }

        if (_profiles.Any((p => p.Name == name)))
        {
            Debug.LogError($"A profile with the name {name} already exists.");
            return false;
        }

        return true;
    }

    public ScriptDefineSymbolManager.IProfile GetProfile(string name)
    {
        return _profiles.Find((p) => p.Name == name);
    }

    public void RenameProfile(ScriptDefineSymbolManager.IProfile profile, string newName)
    {
        if (!IsProfileNameValid(name))
        {
            return;
        }

        if (profile is Profile p)
        {
            p.Name = newName;
        }
    }

    public bool DeleteProfile(ScriptDefineSymbolManager.IProfile profile)
    {
        return _profiles.Remove(profile as Profile);
    }

    #region Asset management
    private const string ASSET_PATH = "Assets/ScriptableObjects/Editor/ScriptDefineSymbolManagerSettings.asset";

    internal static ScriptDefineSymbolManagerSettings GetOrCreateSettings()
    {
        return AssetDatabaseX.LoadOrCreateScriptableObjectAsset<ScriptDefineSymbolManagerSettings>(ASSET_PATH);
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetOrCreateSettings());
    }
    #endregion
}
