using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorX;
using UnityEngine;
using UnityEngineX;

[CustomEditor(typeof(GameActionAuth))]
public class GameActionAuthEditor : Editor
{
    private static string[] s_availableTypeNames;
    private static Type[] s_availableTypes;
    // key : setting type / value : auth type
    private static GUIStyle s_primaryTitleFontStyle = null;
    private static GUIStyle s_secondaryTitleFontStyle = null;

    private SerializedProperty _gameActionProp;
    private SerializedProperty _gameActionSettingsProp;

    private SerializedProperty _sfxProp;
    private SerializedProperty _animationConditionProp;
    private SerializedProperty _animationProp;
    private SerializedProperty _surveyProp;

    private void OnEnable()
    {
        _gameActionProp = serializedObject.FindProperty(nameof(GameActionAuth.Value));
        _gameActionSettingsProp = serializedObject.FindProperty(nameof(GameActionAuth.GameActionSettings));
        
        _sfxProp = serializedObject.FindProperty(nameof(GameActionAuth.SfxOnUse));
        _animationConditionProp = serializedObject.FindProperty(nameof(GameActionAuth.PlayAnimation));
        _animationProp = serializedObject.FindProperty(nameof(GameActionAuth.Animation));
        _surveyProp = serializedObject.FindProperty(nameof(GameActionAuth.CustomSurveys));

        InitStaticData();
    }

    public override void OnInspectorGUI()
    {
        var castedTarget = (GameActionAuth)target;

        EditorGUI.BeginChangeCheck();

        DrawPrimaryTitle("Simulation");

        GUIContent label = EditorGUIUtilityX.TempContent("Game Action Type");
        EditorGUILayoutX.SearchablePopupString(_gameActionProp, label, s_availableTypeNames);

        if (_gameActionProp.stringValue != "")
        {
            Type gameActionType = TypeUtility.FindType(_gameActionProp.stringValue, false);
            if (gameActionType != null)
            {
                Type[] settingAuthTypes = GameActionSettingAuthBase.GetRequiredSettingAuthTypes(gameActionType);

                // Remove extra setting in property list
                for (int i = _gameActionSettingsProp.arraySize - 1; i >= 0; i--)
                {
                    if (castedTarget.GameActionSettings[i] == null || !settingAuthTypes.Contains(castedTarget.GameActionSettings[i].GetType()))
                    {
                        _gameActionSettingsProp.DeleteArrayElementAtIndex(i);
                        castedTarget.GameActionSettings.RemoveAt(i);
                    }
                }

                // Add missing setting to property list
                for (int i = 0; i < settingAuthTypes.Length; i++)
                {
                    Type currentAuthSettingType = settingAuthTypes[i];
                    if (currentAuthSettingType == null)
                        continue;

                    if (!castedTarget.GameActionSettings.Any((x) => x.GetType() == currentAuthSettingType))
                    {
                        _gameActionSettingsProp.InsertArrayElementAtIndex(_gameActionSettingsProp.arraySize);
                        SerializedProperty currentGameActionSetting = _gameActionSettingsProp.GetArrayElementAtIndex(_gameActionSettingsProp.arraySize - 1);
                        var newAuthSetting = Activator.CreateInstance(currentAuthSettingType);
                        currentGameActionSetting.managedReferenceValue = newAuthSetting;
                        castedTarget.GameActionSettings.Add((GameActionSettingAuthBase)newAuthSetting);
                    }
                }

                for (int i = 0; i < _gameActionSettingsProp.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(_gameActionSettingsProp.GetArrayElementAtIndex(i));
                }
            }
        }

        DrawLine(10);

        DrawPrimaryTitle("Presentation");

        EditorGUILayout.PropertyField(_sfxProp);
        EditorGUILayout.PropertyField(_animationConditionProp);
        if (_animationConditionProp.boolValue)
            EditorGUILayout.PropertyField(_animationProp);
        EditorGUILayout.PropertyField(_surveyProp);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    public void DrawLine(int size = 1)
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        Rect rect = EditorGUILayout.GetControlRect(false, size);
        rect.height = size;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

        EditorGUILayout.Space();
    }

    public void DrawPrimaryTitle(string titleText)
    {
        GUILayout.Label(titleText, s_primaryTitleFontStyle);
    }

    public void DrawSecondaryTitle(string titleText)
    {
        GUILayout.Label(titleText, s_secondaryTitleFontStyle);
    }

    private static void InitStaticData()
    {
        if (s_availableTypes == null)
        {
            s_availableTypes = TypeUtility.GetTypesDerivedFrom(typeof(GameAction)).ToArray();
            s_availableTypeNames = s_availableTypes.Where(t => !t.IsAbstract).Select(t => t.Name).ToArray();
        }

        if (s_primaryTitleFontStyle == null)
        {
            s_primaryTitleFontStyle = new GUIStyle();
            s_primaryTitleFontStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            s_primaryTitleFontStyle.fontSize = 20;
            s_primaryTitleFontStyle.fontStyle = FontStyle.Bold;
        }

        if (s_secondaryTitleFontStyle == null)
        {
            s_secondaryTitleFontStyle = new GUIStyle();
            s_secondaryTitleFontStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            s_secondaryTitleFontStyle.fontSize = 15;
            s_secondaryTitleFontStyle.fontStyle = FontStyle.Bold;
        }
    }
}