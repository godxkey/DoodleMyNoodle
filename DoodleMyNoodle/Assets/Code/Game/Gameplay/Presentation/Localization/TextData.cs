using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct TextData
{
    public static LogChannel LogChannel = Log.CreateChannel("Localization", activeByDefault: true);

    [SerializeField] private string _string;
    [SerializeField] private bool _useLocID;
    [SerializeField] private string _locID;

    private List<(string tag, string replacement)> _tagReplacements;

    public static TextData FromLocId(string locId, string failedLocText = null)
    {
        TextData newTextData = new TextData();
        newTextData._locID = locId;
        newTextData._string = failedLocText;
        newTextData._useLocID = true;
        return newTextData;
    }

    public static TextData FromRawString(string text)
    {
        TextData newLocalizedTextData = new TextData();
        newLocalizedTextData._string = text;
        newLocalizedTextData._useLocID = false;
        return newLocalizedTextData;
    }

    public static TextData Empty => FromRawString(string.Empty);
    public static TextData Null => default;

    public void AddTagReplacement(string tag, string replacement)
    {
        if (_tagReplacements == null)
            _tagReplacements = new List<(string tag, string replacement)>();

        _tagReplacements.Add((tag, replacement));
    }

    public override string ToString()
    {
        string outputString = null;
        if (_useLocID)
        {
            if (!LocalizationManager.Instance.GetLocalizedText(_locID, out outputString))
            {
                outputString = "#" + _string;
                Log.Info(LogChannel, $"No Localized Text Found for ID {_locID}, using failedLocText");
            }
        }
        else
        {
            outputString = _string;
        }

        if (_tagReplacements != null && _tagReplacements.Count > 0)
        {
            foreach (var tag in _tagReplacements)
            {
                outputString.Replace(tag.tag, tag.replacement);
            }
        }

        return outputString;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(TextData))]
public class TextDataDrawer : PropertyDrawer
{
    private string _lastID = "";
    private string _lastPreview = "";

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 4 + 6;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var titlePos = new Rect(position.x, position.y, position.width, 16);
        EditorGUI.LabelField(titlePos, new GUIContent("Text Data"));

        EditorGUI.indentLevel++;

        var locIDFieldPos = new Rect(position.x + 100, position.y + 20, position.width, 16);
        var defaultStringFieldPos = new Rect(position.x + 100, position.y + 40, position.width, 16);
        var previewLabelPos = new Rect(position.x + 100, position.y + 60, position.width, 16);

        var locIDFieldLabelPos = new Rect(position.x, position.y + 20, position.width, 16);
        var defaultStringFieldLabelPos = new Rect(position.x, position.y + 40, position.width, 16);
        var previewDisplayPos = new Rect(position.x, position.y + 60, position.width, 16);

        property.FindPropertyRelative("_useLocID").boolValue = true;

        position = EditorGUI.PrefixLabel(locIDFieldLabelPos, new GUIContent("Localization ID :"));
        EditorGUI.PropertyField(locIDFieldPos, property.FindPropertyRelative("_locID"), GUIContent.none);

        position = EditorGUI.PrefixLabel(defaultStringFieldLabelPos, new GUIContent("Default Text :"));
        EditorGUI.PropertyField(defaultStringFieldPos, property.FindPropertyRelative("_string"), GUIContent.none);

        position = EditorGUI.PrefixLabel(previewDisplayPos, new GUIContent("Preview :"));
        string currentID = property.FindPropertyRelative("_locID").stringValue;
        if (currentID != _lastID)
        {
            _lastPreview = GetPreviewText(property.FindPropertyRelative("_locID").stringValue);
            EditorGUI.LabelField(previewLabelPos, _lastPreview);
        }
        else
        {
            EditorGUI.LabelField(previewLabelPos, _lastPreview);
        }
        _lastID = currentID;

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

    private string GetPreviewText(string ID)
    {
        string[] settingsGUID = AssetDatabase.FindAssets(LocalizationManager.LOCALIZATION_SETTING_FILENAME, null);
        if (settingsGUID.Length > 0)
        {
            string settingPath = AssetDatabase.GUIDToAssetPath(settingsGUID[0]);
            LocalizationSettings localizationSettings = (LocalizationSettings)AssetDatabase.LoadAssetAtPath(settingPath, typeof(LocalizationSettings));
            if (localizationSettings.GetLocalizedText("English", ID, out string resultLocalizedText))
            {
                return resultLocalizedText;
            }
        }

        return "N/A";
    }
}
#endif