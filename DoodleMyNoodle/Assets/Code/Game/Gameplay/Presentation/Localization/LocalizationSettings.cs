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

    public bool GetLocalizedText(string Language, string ID, out string ResultLocalizedText)
    {
        LocalizationData localizationData = FindData(ID);
        if (localizationData != null)
        {
            if (!Languages.Contains(Language))
                Debug.LogError("Language you are trying to access doesn't exist");

            int index = Languages.IndexOf(Language);
            if (index >= 0)
            {
                if (localizationData.Localization.Count > index)
                {
                    ResultLocalizedText = localizationData.Localization[index];
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

        ResultLocalizedText = "N/A";
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
        foreach (LocalizationData Localization in Localizations)
        {
            if (Localization.ID == ID)
            {
                return Localization;
            }
        }

        return null;
    }
}