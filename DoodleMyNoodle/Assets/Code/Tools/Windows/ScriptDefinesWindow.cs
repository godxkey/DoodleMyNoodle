using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ScriptDefinesWindow : EditorWindow
{
    ScriptDefinesWindowContent content;

    void OnEnable()
    {
        content = new ScriptDefinesWindowContent();
    }

    void OnGUI()
    {
        content.OnGUI();
    }

    // Open the window
    [MenuItem("Tools/Pipeline/Script Defines")]
    public static void ShowWindow()
    {
        GetWindow<ScriptDefinesWindow>(false, "Script Defines", true);
    }
}

public class ScriptDefinesWindowContent
{
    bool[] symbolValues = new bool[(int)Symbol.max];

    BuildTargetGroup BuildTargetGroup => BuildTargetGroup.Standalone;

    public ScriptDefinesWindowContent()
    {
        GetSymbols();
    }

    public void OnGUI()
    {
        for (int i = 0; i < symbolValues.Length; i++)
        {
            bool isMandatory = IsSymbolMandatory((Symbol)i);
            GUI.enabled = !isMandatory;
            symbolValues[i] = GUILayout.Toggle(symbolValues[i] || isMandatory, SymbolToString((Symbol)i));
            GUI.enabled = true;
        }

        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Refresh Symbols"))
        {
            GetSymbols();
        }
        if(GUILayout.Button("Apply Symbols"))
        {
            ApplySymbols();
        }
        GUILayout.EndHorizontal();

        GUI.enabled = false;
        GUILayout.TextField(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup));
        GUI.enabled = true;
    }

    void GetSymbols()
    {
        // turn symbol values OFF
        for (int i = 0; i < symbolValues.Length; i++)
        {
            symbolValues[i] = false;
        }

        // the symbols are all grouped into 1 string. We must parse the string
        string batchedSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup);
        string[] splitSymbols = batchedSymbols.Split(';');

        // turn symbol values ON, one by one
        foreach (var text in splitSymbols)
        {
            if (text.Length == 0)
                continue;

            Symbol sym = StringToSymbol(text);
            if (sym != Symbol.none)
            {
                symbolValues[(int)sym] = true;
            }
            else
            {
                Debug.LogWarning("Unknown symbol found in the PlayerSettings: " + text + ". It'll ignored and overwitten.");
            }
        }

    }

    void ApplySymbols()
    {
        StringBuilder batchedSymbols = new StringBuilder();

        for (int i = 0; i < symbolValues.Length; i++)
        {
            if (symbolValues[i]) // if symbol value is true, add it in!
            {
                batchedSymbols.Append(SymbolToString((Symbol)i));
                batchedSymbols.Append(';');
            }
        }

        batchedSymbols.Remove(batchedSymbols.Length - 1, 1); // remove last ; character

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup, batchedSymbols.ToString());

        GetSymbols();
    }

    ////////////////////////////////////////////////////////////////////////////////////////
    //      Symbol definitions                                 
    ////////////////////////////////////////////////////////////////////////////////////////
    enum Symbol
    {
        BOLT_CLOUD,
        DEBUG_BUILD,
        max,
        none
    }

    string SymbolToString(Symbol symbol)
    {
        return symbol.ToString();
    }

    Symbol StringToSymbol(string symbol)
    {
        Symbol result;
        if (Enum.TryParse(symbol, out result))
        {
            return result;
        }
        else
        {
            return Symbol.none;
        }
    }

    bool IsSymbolMandatory(Symbol symbol)
    {
        if(symbol == Symbol.BOLT_CLOUD)
        {
            return true;
        }
        return false;
    }
}