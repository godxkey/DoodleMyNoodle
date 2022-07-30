using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct TextData : IEquatable<TextData>
{
    public static LogChannel LogChannel = Log.CreateChannel("Localization", activeByDefault: true);

    [SerializeField] private string _string;
    [SerializeField] private Flags _flags;
    [SerializeField] private int _locID;

    public enum Flags : byte
    {
        None = 0,
        Unlocalized = 1 << 0,
    }

    private List<(string tag, string replacement)> _tagReplacements;

    public static TextData Localized(int locId, string failedLocText = null)
    {
        return new TextData()
        {
            _locID = locId,
            _string = failedLocText,
            _flags = Flags.None,
        };
    }

    public static TextData String(string text)
    {
        return new TextData()
        {
            _string = text,
            _flags = Flags.Unlocalized,
        };
    }

    public static TextData Value(int value) => String(value.ToString()); // todo: optimize this for no allocs)
    public static TextData Value(float value) => String(value.ToString()); // todo: optimize this for no allocs)
    public static TextData Value(DateTime value) => String(value.ToString()); // todo: optimize this for no allocs)
    public static TextData Value(TimeSpan value) => String(value.ToString()); // todo: optimize this for no allocs)
    public static TextData Empty => String(string.Empty);
    public static TextData Null => default;

    public void AddTagReplacement(string tag, string replacement)
    {
        if (_tagReplacements == null)
            _tagReplacements = new List<(string tag, string replacement)>();

        _tagReplacements.Add((tag, replacement));
    }

    public override string ToString()
    {
        string outputString;
        //if ((int)(_flags & Flags.Unlocalized) == 0) // localization currently unsupported
        //{
        //    if (!LocalizationManager.Instance.GetLocalizedText(_locID, out outputString))
        //    {
        //        outputString = _string;
        //        //Log.Info(LogChannel, $"No Localized Text Found for ID {_locID}, using failedLocText");
        //    }
        //}
        //else
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

    public bool Equals(TextData other)
    {
        return _flags == other._flags
            && _locID == other._locID
            && _string == other._string
            && _tagReplacements == other._tagReplacements;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(TextData))]
public class TextDataDrawer : PropertyDrawer
{
    private struct Properties
    {
        public SerializedProperty LocID;
        public SerializedProperty String;
        public SerializedProperty Flags;

        public Properties(SerializedProperty root)
        {
            LocID = root.FindPropertyRelative("_locID");
            String = root.FindPropertyRelative("_string");
            Flags = root.FindPropertyRelative("_flags");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 4 + 6;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Properties properties = new Properties(property);

        int lastID = properties.LocID.intValue;

        EditorGUI.BeginProperty(position, label, property);

        var titlePos = new Rect(position.x, position.y, position.width, 16);
        EditorGUI.LabelField(titlePos, new GUIContent("Text Data"));

        EditorGUI.indentLevel++;

        var locIDFieldPos = new Rect(position.x, position.y + 20, position.width, EditorGUIUtility.singleLineHeight);
        var defaultStringFieldPos = new Rect(position.x, position.y + 40, position.width, EditorGUIUtility.singleLineHeight);
        var previewLabelPos = new Rect(position.x, position.y + 60, position.width, EditorGUIUtility.singleLineHeight);

        properties.Flags.enumValueFlag = (int)TextData.Flags.None;

        EditorGUI.PropertyField(locIDFieldPos, properties.LocID, new GUIContent("Localization ID :"));
        EditorGUI.PropertyField(defaultStringFieldPos, properties.String, new GUIContent("Default Text :"));
        EditorGUI.LabelField(previewLabelPos, new GUIContent("Preview :"), new GUIContent(GetPreviewText(properties.LocID.intValue)));

        EditorGUI.indentLevel--;

        EditorGUI.EndProperty();
    }

    private string GetPreviewText(int ID)
    {
        // unsupported for now

        //string[] settingsGUID = AssetDatabase.FindAssets(LocalizationManager.LOCALIZATION_SETTING_FILENAME, null);
        //if (settingsGUID.Length > 0)
        //{
        //    string settingPath = AssetDatabase.GUIDToAssetPath(settingsGUID[0]);
        //    LocalizationSettings localizationSettings = (LocalizationSettings)AssetDatabase.LoadAssetAtPath(settingPath, typeof(LocalizationSettings));
        //    if (localizationSettings.GetLocalizedText("English", "", out string resultLocalizedText))
        //    {
        //        return resultLocalizedText;
        //    }
        //}

        return "N/A";
    }
}
#endif