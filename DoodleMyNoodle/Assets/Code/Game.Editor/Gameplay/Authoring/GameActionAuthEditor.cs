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
    private static string[] s_availableTypeDisplayNames;
    private static Type[] s_availableTypes;
    // key : setting type / value : auth type
    private static GUIStyle s_primaryTitleFontStyle = null;
    private static GUIStyle s_secondaryTitleFontStyle = null;

    private SerializedProperty _propGameAction;
    private SerializedProperty _propGameActionSettings;

    private SerializedProperty _propUseInstigatorAsTarget;
    private SerializedProperty _propInstigatorAsTargetType;

    private SerializedProperty _propSfxOnUse;
    private SerializedProperty _propSfxOnPrepareUse;
    private SerializedProperty _propAnimation;
    private SerializedProperty _propSurveys;
    private SerializedProperty _propInstigatorVFX;
    private SerializedProperty _propTargetsVFXProp;

    private void OnEnable()
    {
        _propGameAction = serializedObject.FindProperty(nameof(GameActionAuth.Value));
        _propGameActionSettings = serializedObject.FindProperty(nameof(GameActionAuth.GameActionSettings));

        _propUseInstigatorAsTarget = serializedObject.FindProperty(nameof(GameActionAuth.UseInstigatorAsTarget));
        _propInstigatorAsTargetType = serializedObject.FindProperty(nameof(GameActionAuth.InstigatorAsTargetType));

        _propSfxOnUse = serializedObject.FindProperty(nameof(GameActionAuth.SfxOnUse));
        _propSfxOnPrepareUse = serializedObject.FindProperty(nameof(GameActionAuth.SfxOnPrepareUse));
        _propAnimation = serializedObject.FindProperty(nameof(GameActionAuth.Animation));
        _propSurveys = serializedObject.FindProperty(nameof(GameActionAuth.Surveys));

        _propInstigatorVFX = serializedObject.FindProperty(nameof(GameActionAuth.InstigatorVFX));
        _propTargetsVFXProp = serializedObject.FindProperty(nameof(GameActionAuth.TargetsVFX));

        InitStaticData();
    }

    public override void OnInspectorGUI()
    {
        var castedTarget = (GameActionAuth)target;

        EditorGUI.BeginChangeCheck();

        DrawPrimaryTitle("Simulation");

        GUIContent label = EditorGUIUtilityX.TempContent("Game Action Type");
        int newIndex = EditorGUILayoutX.SearchablePopup(label, Array.IndexOf(s_availableTypeNames, _propGameAction.stringValue), s_availableTypeDisplayNames);
        _propGameAction.stringValue = s_availableTypeNames.IsValidIndex(newIndex) ? s_availableTypeNames[newIndex] : "";

        if (_propGameAction.stringValue != "")
        {
            Type gameActionType = TypeUtility.FindType(_propGameAction.stringValue, false);
            if (gameActionType != null)
            {
                Type[] settingAuthTypes = GameActionSettingAuthBase.GetRequiredSettingAuthTypes(gameActionType);

                // Remove extra setting in property list
                for (int i = _propGameActionSettings.arraySize - 1; i >= 0; i--)
                {
                    if (castedTarget.GameActionSettings[i] == null || !settingAuthTypes.Contains(castedTarget.GameActionSettings[i].GetType()))
                    {
                        _propGameActionSettings.DeleteArrayElementAtIndex(i);
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
                        _propGameActionSettings.InsertArrayElementAtIndex(_propGameActionSettings.arraySize);
                        SerializedProperty currentGameActionSetting = _propGameActionSettings.GetArrayElementAtIndex(_propGameActionSettings.arraySize - 1);
                        var newAuthSetting = Activator.CreateInstance(currentAuthSettingType);
                        currentGameActionSetting.managedReferenceValue = newAuthSetting;
                        castedTarget.GameActionSettings.Add((GameActionSettingAuthBase)newAuthSetting);
                    }
                }

                for (int i = 0; i < _propGameActionSettings.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(_propGameActionSettings.GetArrayElementAtIndex(i));
                }
            }
        }

        EditorGUILayout.PropertyField(_propUseInstigatorAsTarget);
        if (_propUseInstigatorAsTarget.boolValue)
        {
            EditorGUILayout.PropertyField(_propInstigatorAsTargetType);
        }

        DrawLine(10);

        DrawPrimaryTitle("Presentation");

        EditorGUILayout.PropertyField(_propSfxOnPrepareUse);
        EditorGUILayout.PropertyField(_propSfxOnUse);
        EditorGUILayout.PropertyField(_propAnimation);
        EditorGUILayout.PropertyField(_propSurveys);

        EditorGUILayout.PropertyField(_propInstigatorVFX);
        EditorGUILayout.PropertyField(_propTargetsVFXProp);

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
            s_availableTypeNames = s_availableTypes.Where(t => !t.IsAbstract).Select(t => t.FullName).ToArray();
            s_availableTypeDisplayNames = s_availableTypeNames.Select(n => n.Replace('+', '.')).ToArray();
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