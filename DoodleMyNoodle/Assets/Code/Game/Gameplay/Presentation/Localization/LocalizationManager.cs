using System;
using UnityEngine;
using UnityEngineX;

public class LocalizationManager : GameSystem<LocalizationManager>
{
    public string DefaultLanguage = "English";
    public LocalizationSettings LocalizationSettings;

    private const string SELECTED_LANGUAGE_KEY = "N/A";
    private string _currentlySelectedLanguage = "N/A";

    public const string LOCALIZATION_SETTING_FILENAME = "LocalizationSettingsAsset";

    protected override void Awake()
    {
        if (!Load())
        {
            ChangeSelectedLanguage(DefaultLanguage);
        }

        base.Awake();
    }

    private bool Load()
    {
        if (PlayerPrefs.HasKey(SELECTED_LANGUAGE_KEY))
        {
            _currentlySelectedLanguage = PlayerPrefs.GetString(SELECTED_LANGUAGE_KEY);
            return true;
        }

        return false;
    }

    private void Save()
    {
        PlayerPrefs.SetString(SELECTED_LANGUAGE_KEY, _currentlySelectedLanguage);
        PlayerPrefs.Save();
    }

    public void ChangeSelectedLanguage(string newLanguage)
    {
        _currentlySelectedLanguage = newLanguage;
        Save();
    }

    public bool GetLocalizedText(string id, out string result)
    {
        return LocalizationSettings.GetLocalizedText(_currentlySelectedLanguage, id, out result);
    }
}