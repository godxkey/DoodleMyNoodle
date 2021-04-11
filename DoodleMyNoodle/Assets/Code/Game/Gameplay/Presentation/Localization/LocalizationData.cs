using System;
using System.Collections.Generic;

[Serializable]
public class LocalizationData
{
    public string ID = "NEW_ID";
    public List<string> Localization = new List<string>();

    public LocalizationData(int LanguageCount)
    {
        Localization = new List<string>(LanguageCount);
        for (int i = 0; i < LanguageCount; i++)
            Localization.Add("");
    }
}