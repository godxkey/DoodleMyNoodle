using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEditor;
using UnityEngineX;

public partial class LocalizationWindow : EditorWindow
{
    List<LocalizationData> Localizations = new List<LocalizationData>();
    List<string> Languages = new List<string>();

    LocalizationSettings SavedSettings;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Tools/Localization")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        LocalizationWindow window = (LocalizationWindow)EditorWindow.GetWindow(typeof(LocalizationWindow));

        string[] settingsGUID = AssetDatabase.FindAssets(LocalizationManager.LOCALIZATION_SETTING_FILENAME, null);
        if (settingsGUID.Length > 0)
        {
            string settingPath = AssetDatabase.GUIDToAssetPath(settingsGUID[0]);
            window.SavedSettings = (LocalizationSettings)AssetDatabase.LoadAssetAtPath(settingPath, typeof(LocalizationSettings));

            if (window.SavedSettings)
            {
                window.Localizations = window.SavedSettings.Localizations;
                window.Languages = window.SavedSettings.Languages;
                Debug.Log("Localization Settings successfully loaded");
            }
            else
            {
                Debug.LogError("Localization Settings couldn't be found. Localization won't be saved");
            }
        }
        else
        {
            Debug.LogError("Localization Settings couldn't be found. Localization won't be saved");
        }

        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Localization Settings", EditorStyles.boldLabel);

        if (GUILayout.Button("Import CSV"))
        {
            // ToDo
        }

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    if (GUILayout.Button("Clear All"))
                    {
                        Languages.Add("New Language");
                    }
                }
                GUILayout.EndVertical();

                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    if (GUILayout.Button("Add Language"))
                    {
                        Languages.Add("New Language");
                    }
                }
                GUILayout.EndVertical();

                EditorGUILayout.TextField("IDs");
            }
            GUILayout.EndVertical();

            int languageIndexToRemove = -1;
            for (int i = 0; i < Languages.Count; i++)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    Languages[i] = EditorGUILayout.TextField(Languages[i]);

                    GUILayout.BeginHorizontal(EditorStyles.helpBox);
                    {
                        if (GUILayout.Button("Import CSV"))
                        {
                            // ToDo
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal(EditorStyles.helpBox);
                    {
                        if (GUILayout.Button("Remove"))
                        {
                            languageIndexToRemove = i;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }

            if (languageIndexToRemove >= 0)
                Languages.RemoveAt(languageIndexToRemove);
        }
        GUILayout.EndHorizontal();

        DrawLine(2);

        List<LocalizationData> TempLocalizations = new List<LocalizationData>();
        int indexToRemove = -1;
        for (int i = 0; i < Localizations.Count; i++)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                LocalizationData CurrentRow = Localizations[i];
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    LocalizationData currentValue = new LocalizationData(Languages.Count);

                    currentValue.ID = EditorGUILayout.TextField(CurrentRow.ID);

                    for (int j = 0; j < Languages.Count; j++)
                    {
                        string currentLocalizationText = "N/A";
                        if (CurrentRow.Localization.Count > j)
                        {
                            currentLocalizationText = CurrentRow.Localization[j];
                        }

                        currentValue.Localization[j] = EditorGUILayout.TextField(currentLocalizationText);
                    }

                    TempLocalizations.Add(currentValue);
                }
                GUILayout.EndHorizontal();

                if (GUILayout.Button("Remove Entry"))
                {
                    indexToRemove = i;
                }
            }
            GUILayout.EndVertical();
        }

        if (indexToRemove >= 0)
            TempLocalizations.RemoveAt(indexToRemove);

        Localizations = TempLocalizations;

        DrawLine(2);

        if (GUILayout.Button("Add Entry"))
        {
            Localizations.Add(new LocalizationData(Languages.Count));
        }

        if (SavedSettings != null)
        {
            SavedSettings.Save(Localizations, Languages);
        }
        else
        {
            Debug.LogError("Localization Settings couldn't be found. Localization won't be saved");
        }
    }

    public void DrawLine(int size = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, size);
        rect.height = size;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}