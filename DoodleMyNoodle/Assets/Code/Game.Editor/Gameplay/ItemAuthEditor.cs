using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorX;
using UnityEngine;
using UnityEngineX;

[CustomEditor(typeof(ItemAuth))]
public class ItemAuthEditor : Editor
{
    private static string[] s_availableTypeNames;
    private static Type[] s_availableTypes;
    // key : setting type / value : auth type
    private static Dictionary<Type, Type> s_gameActionSettingAuthTypes;
    private static GUIStyle s_primaryTitleFontStyle = null;
    private static GUIStyle s_secondaryTitleFontStyle = null;

    private SerializedProperty _gameActionProp;
    private SerializedProperty _gameActionSettingsProp;
    private SerializedProperty _cooldownProp;
    private SerializedProperty _cooldownDurationProp;
    private SerializedProperty _stackableProp;
    private SerializedProperty _apCostProp;
    private SerializedProperty _iconProp;
    private SerializedProperty _nameProp;
    private SerializedProperty _effectDescriptionProp;
    private SerializedProperty _sfxProp;
    private SerializedProperty _animationConditionProp;
    private SerializedProperty _animationProp;
    private SerializedProperty _surveyProp;
    private SerializedProperty _hideInInventory;
    private SerializedProperty _canBeUsedAtAnytime;

    private void OnEnable()
    {
        _gameActionProp = serializedObject.FindProperty(nameof(ItemAuth.Value));
        _gameActionSettingsProp = serializedObject.FindProperty(nameof(ItemAuth.GameActionSettings));
        _cooldownProp = serializedObject.FindProperty(nameof(ItemAuth.CooldownType));
        _cooldownDurationProp = serializedObject.FindProperty(nameof(ItemAuth.CooldownDuration));
        _stackableProp = serializedObject.FindProperty(nameof(ItemAuth.IsStackable));
        _apCostProp = serializedObject.FindProperty(nameof(ItemAuth.ApCost));
        _iconProp = serializedObject.FindProperty(nameof(ItemAuth.Icon));
        _nameProp = serializedObject.FindProperty(nameof(ItemAuth.Name));
        _effectDescriptionProp = serializedObject.FindProperty(nameof(ItemAuth.EffectDescription));
        _sfxProp = serializedObject.FindProperty(nameof(ItemAuth.SfxOnUse));
        _animationConditionProp = serializedObject.FindProperty(nameof(ItemAuth.PlayAnimation));
        _animationProp = serializedObject.FindProperty(nameof(ItemAuth.Animation));
        _surveyProp = serializedObject.FindProperty(nameof(ItemAuth.CustomSurveys));
        _hideInInventory = serializedObject.FindProperty(nameof(ItemAuth.HideInInventory));
        _canBeUsedAtAnytime = serializedObject.FindProperty(nameof(ItemAuth.CanBeUsedAtAnytime));

        InitStaticData();
    }

    public override void OnInspectorGUI()
    {
        var castedTarget = (ItemAuth)target;

        EditorGUI.BeginChangeCheck();

        DrawPrimaryTitle("Simulation");

        EditorGUILayout.PropertyField(_apCostProp);
        EditorGUILayout.PropertyField(_cooldownProp);
        if(castedTarget.HasCooldown)
            EditorGUILayout.PropertyField(_cooldownDurationProp);
        EditorGUILayout.PropertyField(_stackableProp);

        GUIContent label = EditorGUIUtilityX.TempContent("Game Action Type");
        EditorGUILayoutX.SearchablePopupString(_gameActionProp, label, s_availableTypeNames);

        if (_gameActionProp.stringValue != "")
        {
            Type gameActionType = TypeUtility.FindType(_gameActionProp.stringValue, false);
            if (gameActionType != null)
            {
                GameAction selectedGameAction = (GameAction)Activator.CreateInstance(gameActionType);
                Type[] gameActionAuthSettingTypes = selectedGameAction.GetRequiredSettingTypes().Select((simType) =>
                {
                    if (!s_gameActionSettingAuthTypes.TryGetValue(simType, out Type currentRequiredAuthSettingType))
                    {
                        Debug.LogError($"Game Action Setting Auth Type doesn't exist for {simType}");
                        return null;
                    }
                    else
                    {
                        return currentRequiredAuthSettingType;
                    }
                }).ToArray();

                // Remove extra setting in property list
                for (int i = _gameActionSettingsProp.arraySize - 1; i >= 0; i--)
                {
                    if (castedTarget.GameActionSettings[i] == null || !gameActionAuthSettingTypes.Contains(castedTarget.GameActionSettings[i].GetType()))
                    {
                        _gameActionSettingsProp.DeleteArrayElementAtIndex(i);
                        castedTarget.GameActionSettings.RemoveAt(i);
                    }
                }

                // Add missing setting to property list
                for (int i = 0; i < gameActionAuthSettingTypes.Length; i++)
                {
                    Type currentAuthSettingType = gameActionAuthSettingTypes[i];
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

        EditorGUILayout.PropertyField(_iconProp);
        EditorGUILayout.PropertyField(_nameProp);
        EditorGUILayout.PropertyField(_effectDescriptionProp);
        EditorGUILayout.PropertyField(_sfxProp);
        EditorGUILayout.PropertyField(_animationConditionProp);
        if (_animationConditionProp.boolValue)
            EditorGUILayout.PropertyField(_animationProp);
        EditorGUILayout.PropertyField(_surveyProp);
        EditorGUILayout.PropertyField(_hideInInventory);
        EditorGUILayout.PropertyField(_canBeUsedAtAnytime);

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
            s_availableTypeNames = s_availableTypes.Select(t => t.Name).ToArray();
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

        if (s_gameActionSettingAuthTypes == null)
        {
            s_gameActionSettingAuthTypes = new Dictionary<Type, Type>();
        }

        Type[] gameActionSettingAuthTypes = TypeUtility.GetTypesWithAttribute(typeof(GameActionSettingAuthAttribute)).ToArray();
        foreach (Type gameActionSettingAuthType in gameActionSettingAuthTypes)
        {
            GameActionSettingAuthAttribute attribute = (GameActionSettingAuthAttribute)gameActionSettingAuthType.GetCustomAttribute(typeof(GameActionSettingAuthAttribute));
            if (!s_gameActionSettingAuthTypes.ContainsKey(attribute.SettingType))
            {
                s_gameActionSettingAuthTypes.Add(attribute.SettingType, gameActionSettingAuthType);
            }
        }
    }
}