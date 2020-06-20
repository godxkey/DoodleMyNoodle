using System;
using System.Collections;
using UnityEditor;

public static partial class ScriptDefineSymbolManager
{
    public interface IProfile
    {
        string Name { get; }
        ReadOnlyList<string> DefinedSymbols { get; }
    }

    public static ReadOnlyListDynamic<IProfile> Profiles => GetSettings().Profiles;
    public static ReadOnlyList<string> Symbols => GetSettings().Symbols;

    public static void CreateSymbol(string symbol)
    {
        GetSettings().CreateSymbol(symbol);
        EditorUtility.SetDirty(GetSettings());
    }

    public static void DeleteSymbol(string symbol)
    {
        GetSettings().DeleteSymbol(symbol);
        EditorUtility.SetDirty(GetSettings());
    }

    public static void AddSymbolInProfile(string symbol, IProfile profile)
    {
        GetSettings().AddSymbolInProfile(symbol, profile);
        EditorUtility.SetDirty(GetSettings());
    }

    public static bool RemoveSymbolFromProfile(string symbol, IProfile profile)
    {
        bool result =  GetSettings().RemoveSymbolFromProfile(symbol, profile);
        EditorUtility.SetDirty(GetSettings());
        return result;
    }

    public static IProfile CreateProfile(string name)
    {
        var result = GetSettings().CreateProfile(name);
        EditorUtility.SetDirty(GetSettings());
        return result;
    }

    public static IProfile GetProfile(string name)
    {
        return GetSettings().GetProfile(name);
    }

    public static void RenameProfile(IProfile profile, string newName)
    {
        GetSettings().RenameProfile(profile, newName);
        EditorUtility.SetDirty(GetSettings());
    }

    public static bool DeleteProfile(IProfile profile)
    {
        var result = GetSettings().DeleteProfile(profile);
        EditorUtility.SetDirty(GetSettings());
        return result;
    }

    private static ScriptDefineSymbolManagerSettings s_settings;

    private static ScriptDefineSymbolManagerSettings GetSettings()
    {
        if (s_settings == null)
        {
            s_settings = ScriptDefineSymbolManagerSettings.GetOrCreateSettings();
        }

        return s_settings;
    }
}

public static class ScriptDefineSymbolManagerProfileExtensions
{
    public static string GetCombinedSymbols(this ScriptDefineSymbolManager.IProfile profile) => string.Join(";", profile.DefinedSymbols.ToArray());
}