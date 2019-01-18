using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Traduction
{
    string originalText;
    string transcriptText;
}

[System.Serializable]
public class LocalisationData {

    // Make 1 file (data class per language)
    Dictionary<string, List<Traduction>> LanguageData;

	
}
