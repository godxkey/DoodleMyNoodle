using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngineX;

public class ScriptDefineSymbolManagerSettingsEditor
{
    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {
        var provider = new SettingsProvider("Project/Script Define Symbols", SettingsScope.Project)
        {
            guiHandler = OnGUI,
            keywords = new HashSet<string>(new[] { "Symbol", "Symbols", "Define", "Manager", "Script", "Scripting", "Profile", "Build" })
        };

        return provider;
    }

    private static Rect s_createProfileButtonRect;
    private static Rect s_createSymbolButtonRect;
    private static Dictionary<ScriptDefineSymbolManager.IProfile, bool> s_foldStates = new Dictionary<ScriptDefineSymbolManager.IProfile, bool>();
    private static bool s_symbolsFoldState;
    private readonly static Vector2 s_popupWindowSize = new Vector2(275, 45);

    private static void OnGUI(string searchContext)
    {
        //cleanup
        foreach (var profile in s_foldStates.Keys)
        {
            if (!ScriptDefineSymbolManager.Profiles.Contains(profile))
            {
                s_foldStates.Remove(profile);
                break;
            }
        }

        DrawSymbolList();
        DrawProfileList();
    }

    private static void DrawProfileList()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("Profiles", EditorStyles.boldLabel);

        foreach (ScriptDefineSymbolManager.IProfile profile in ScriptDefineSymbolManager.Profiles)
        {
            if (!s_foldStates.ContainsKey(profile))
                s_foldStates.Add(profile, false);

            s_foldStates[profile] = EditorGUILayout.BeginFoldoutHeaderGroup(s_foldStates[profile], profile.Name, menuAction: (Rect rect) =>
            {
                Rect screenRect = GUIUtility.GUIToScreenRect(rect);
                var genericMenu = new GenericMenu();
                genericMenu.AddItem(new GUIContent("Delete"), false, () =>
                {
                    if (EditorUtility.DisplayDialog("Delete Profile", $"Are you sure you want to delete the profile \"{profile.Name}\" ?", "Yes", "Cancel"))
                    {
                        ScriptDefineSymbolManager.DeleteProfile(profile);
                    }
                });
                genericMenu.AddItem(new GUIContent("Rename"), false, () =>
                {
                    Rect p = new Rect(screenRect.position, s_popupWindowSize);
                    var window = EditorWindow.GetWindowWithRect<PopupRenameProfile>(p, true, "Rename Profile");
                    window.position = p;
                    window.Profile = profile;
                });
                genericMenu.AddItem(new GUIContent("Apply To Current Build Target"), false, () =>
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, profile.GetCombinedSymbols());
                });
                genericMenu.ShowAsContext();
            });


            if (s_foldStates[profile])
            {
                List<string> symbolsToAdd = ListPool<string>.Take();
                List<string> symbolsToRemove = ListPool<string>.Take();

                // gui each symbol
                foreach (var symbol in ScriptDefineSymbolManager.Symbols)
                {
                    bool enabled = profile.DefinedSymbols.Contains(symbol);

                    bool newEnabled = GUILayout.Toggle(enabled, symbol);

                    if (enabled != newEnabled)
                    {
                        if (newEnabled)
                            symbolsToAdd.Add(symbol);
                        else
                            symbolsToRemove.Add(symbol);
                    }
                }

                // add/remove symbols
                foreach (var item in symbolsToAdd)
                {
                    ScriptDefineSymbolManager.AddSymbolInProfile(item, profile);
                }
                foreach (var item in symbolsToRemove)
                {
                    ScriptDefineSymbolManager.RemoveSymbolFromProfile(item, profile);
                }

                // release lists
                ListPool<string>.Release(symbolsToRemove);
                ListPool<string>.Release(symbolsToAdd);

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        if (GUILayout.Button("New Profile"))
        {
            Rect rect = new Rect(s_createProfileButtonRect.position + Vector2.up * 60, s_popupWindowSize);
            var window = EditorWindow.GetWindowWithRect<PopupCreateProfile>(rect, true, "Create Profile");
            window.position = rect;
        }

        if (Event.current.type == EventType.Repaint) s_createProfileButtonRect = GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect());

        EditorGUILayout.EndVertical();
    }

    private static void DrawSymbolList()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        s_symbolsFoldState = EditorGUILayout.BeginFoldoutHeaderGroup(s_symbolsFoldState, "Symbols");

        if (s_symbolsFoldState)
        {
            List<string> symbolsToRemove = ListPool<string>.Take();
            foreach (var symbol in ScriptDefineSymbolManager.Symbols)
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("Delete Symbol", $"Are you sure you want to delete the symbol \"{symbol}\" ?", "Yes", "Cancel"))
                    {
                        symbolsToRemove.Add(symbol);
                    }
                }
                EditorGUILayout.LabelField(symbol);

                EditorGUILayout.EndHorizontal();
            }

            foreach (var symbol in symbolsToRemove)
            {
                ScriptDefineSymbolManager.DeleteSymbol(symbol);
            }

            ListPool<string>.Release(symbolsToRemove);
        }

        if (GUILayout.Button("New Symbol"))
        {
            Rect rect = new Rect(s_createSymbolButtonRect.position + Vector2.up * 60, s_popupWindowSize);
            var window = EditorWindow.GetWindowWithRect<PopupCreateSymbol>(rect, true, "Create Symbol");
            window.position = rect;
        }

        if (Event.current.type == EventType.Repaint) s_createSymbolButtonRect = GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect());


        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.EndVertical();
    }

    public class PopupCreateProfile : EditorWindow
    {
        private string _name;

        public void OnGUI()
        {
            _name = EditorGUILayout.TextField("Name", _name);

            if (GUILayout.Button("Create"))
            {
                ScriptDefineSymbolManager.CreateProfile(_name);
                Close();
            }
        }

        private void OnLostFocus()
        {
            Close();
        }
    }

    public class PopupRenameProfile : EditorWindow
    {
        public ScriptDefineSymbolManager.IProfile Profile;

        private string _name;
        private bool _firstUpdate = true;

        public void OnGUI()
        {
            if (_firstUpdate)
            {
                _name = Profile.Name;
                _firstUpdate = false;
            }

            _name = EditorGUILayout.TextField("Name", _name);

            if (GUILayout.Button("Rename"))
            {
                ScriptDefineSymbolManager.RenameProfile(Profile, _name);
                Close();
            }
        }

        private void OnLostFocus()
        {
            Close();
        }
    }

    public class PopupCreateSymbol : EditorWindow
    {
        private string _name;

        public void OnGUI()
        {
            _name = EditorGUILayout.TextField("Name", _name);

            if (GUILayout.Button("Create"))
            {
                ScriptDefineSymbolManager.CreateSymbol(_name);
                Close();
            }
        }

        private void OnLostFocus()
        {
            Close();
        }
    }
}
