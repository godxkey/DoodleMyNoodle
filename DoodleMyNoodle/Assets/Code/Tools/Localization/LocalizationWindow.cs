using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEditor;
using UnityEngineX;
using System.IO;

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

        window.Show();
    }

    private void OnEnable()
    {
        string[] settingsGUID = AssetDatabase.FindAssets(LocalizationManager.LOCALIZATION_SETTING_FILENAME, null);
        if (settingsGUID.Length > 0)
        {
            string settingPath = AssetDatabase.GUIDToAssetPath(settingsGUID[0]);
            SavedSettings = (LocalizationSettings)AssetDatabase.LoadAssetAtPath(settingPath, typeof(LocalizationSettings));

            if (SavedSettings)
            {
                Localizations = SavedSettings.Localizations;
                Languages = SavedSettings.Languages;
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
    }

    void OnGUI()
    {
        Localizations = SavedSettings.Localizations;
        Languages = SavedSettings.Languages;

        GUILayout.Label("Localization Settings", EditorStyles.boldLabel);

        if (GUILayout.Button("Import CSV"))
        {
            string path = EditorUtility.OpenFilePanel("Import CSV", "", "csv");
            if (path.Length != 0)
            {
                byte[] fileBytes = File.ReadAllBytes(path);
                string fileString = System.Text.Encoding.UTF8.GetString(fileBytes);

                ImportCSV(fileString);
                return;
            }
        }

        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    if (GUILayout.Button("Clear All"))
                    {
                        Languages.Clear();
                        Localizations.Clear();
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

    private void ImportCSV(string fileString, bool overwrideAll = true)
    {
        string[] rows = fileString.Split(char.Parse("\n"));

        // Setup languages
        string[] languages = rows[0].Split(char.Parse(","));
        List<string> currentLanguages = new List<string>(languages);
        currentLanguages.RemoveAt(0);

        // Setup localization datas
        List<LocalizationData> NewLocalizations = new List<LocalizationData>();
        for (int r = 1; r < rows.Length; r++)
        {
            string row = rows[r];
            string currentRowCleaned = row.Replace("\r", "");
            string[] elements = currentRowCleaned.Split(char.Parse(","));

            // ID
            LocalizationData newLocalizationData = new LocalizationData(currentLanguages.Count);
            newLocalizationData.ID = elements[0];

            // TRADUCTION
            if (overwrideAll)
            {
                for (int i = 1; i < elements.Length; i++)
                {
                    newLocalizationData.Localization[i-1] = elements[i];
                }

                NewLocalizations.Add(newLocalizationData);
            }
        }

        if (SavedSettings != null)
        {
            SavedSettings.Save(NewLocalizations, currentLanguages);
        }
        else
        {
            Debug.LogError("Localization Settings couldn't be found. Localization won't be saved");
        }
    }
}