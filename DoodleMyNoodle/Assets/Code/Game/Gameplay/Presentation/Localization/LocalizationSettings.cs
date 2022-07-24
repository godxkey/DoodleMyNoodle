using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "CCC/Localization/Settings")]
public class LocalizationSettings : ScriptableObject
{
    public List<string> Languages = new List<string>();
    public List<LocalizationData> Localizations = new List<LocalizationData>();

    public bool GetLocalizedText(string language, string id, out string result)
    {
        LocalizationData localizationData = FindData(id);
        if (localizationData != null)
        {
            if (!Languages.Contains(language))
                Debug.LogError("Language you are trying to access doesn't exist");

            int index = Languages.IndexOf(language);
            if (index >= 0)
            {
                if (localizationData.Localization.Count > index)
                {
                    result = localizationData.Localization[index];
                    return true;
                }
                else
                {
                    Debug.LogError("Missing entry in localization for specified language");
                }
            }
            else
            {
                Debug.LogError("Language invalid");
            }
        }

        result = "N/A";
        return false;
    }

    public void Save(List<LocalizationData> NewLocalizations, List<string> NewLanguages)
    {
        Languages = NewLanguages;
        Localizations = NewLocalizations;
    }

    public int GetLanguageIndex(string language)
    {
        for (int i = 0; i < Languages.Count; i++)
        {
            if (language == Languages[i])
            {
                return i;
            }
        }

        return -1;
    }

    public LocalizationData FindData(string ID)
    {
        foreach (LocalizationData locData in Localizations)
        {
            if (locData.ID == ID)
            {
                return locData;
            }
        }

        return null;
    }
}