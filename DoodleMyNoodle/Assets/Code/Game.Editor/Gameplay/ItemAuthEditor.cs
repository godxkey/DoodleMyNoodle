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
    private static Dictionary<Type,Type> s_gameActionSettingAuthTypes;

    private static GUIStyle s_primaryTitleFontStyle = null;
    private static GUIStyle s_secondaryTitleFontStyle = null;

    private void OnEnable()
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

    public override void OnInspectorGUI()
    {
        var castedTarget = (ItemAuth)target;

        EditorGUI.BeginChangeCheck();

        DrawPrimaryTitle("Simulation");

        DrawLine(2);

        DrawSecondaryTitle("Game Action Settings");

        GUIContent label = EditorGUIUtilityX.TempContent("Game Action Type");
        SerializedProperty gameActionProperty = serializedObject.FindProperty("Value");
        EditorGUILayoutX.SearchablePopupString(gameActionProperty, label, s_availableTypeNames);

        if (gameActionProperty.stringValue != "")
        {
            Type gameActionType = TypeUtility.FindType(gameActionProperty.stringValue, false);
            if (gameActionType != null)
            {
                GameAction selectedGameAction = (GameAction)Activator.CreateInstance(gameActionType);
                SerializedProperty values = serializedObject.FindProperty("GameActionSettings");
                Type[] gameActionAuthSettingTypes = selectedGameAction.GetRequiredSettingTypes().Select((simType)=> 
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
                for (int i = values.arraySize - 1; i >= 0; i--)
                {
                    if (!gameActionAuthSettingTypes.Contains(castedTarget.GameActionSettings[i].GetType()))
                    {
                        values.DeleteArrayElementAtIndex(i);
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
                        values.InsertArrayElementAtIndex(values.arraySize);
                        SerializedProperty currentGameActionSetting = values.GetArrayElementAtIndex(values.arraySize - 1);
                        var newAuthSetting = Activator.CreateInstance(currentAuthSettingType);
                        currentGameActionSetting.managedReferenceValue = newAuthSetting;
                        castedTarget.GameActionSettings.Add((GameActionSettingAuthBase)newAuthSetting);
                    }
                }

                EditorGUILayout.Space();
                for (int i = 0; i < values.arraySize; i++)
                {
                    EditorGUILayout.LabelField(castedTarget.GameActionSettings[i].GetType().ToString(), EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(values.GetArrayElementAtIndex(i));
                    EditorGUILayout.Space();
                }
            }
        }

        DrawSecondaryTitle("Item Settings");

        SerializedProperty hasCooldownSetting = serializedObject.FindProperty("HasCooldown");
        EditorGUILayout.PropertyField(hasCooldownSetting);

        if (hasCooldownSetting.boolValue)
        {
            SerializedProperty cooldownSetting = serializedObject.FindProperty("CooldownAuth");
            EditorGUILayout.PropertyField(cooldownSetting);
        }

        SerializedProperty stackable = serializedObject.FindProperty("IsStackable");
        EditorGUILayout.PropertyField(stackable);

        DrawLine(10);

        DrawPrimaryTitle("Presentation");

        DrawLine(2);

        DrawSecondaryTitle("Description");

        SerializedProperty iconProperty = serializedObject.FindProperty("Icon");
        EditorGUILayout.PropertyField(iconProperty);

        SerializedProperty nameProperty = serializedObject.FindProperty("Name");
        EditorGUILayout.PropertyField(nameProperty);

        SerializedProperty effectDescriptionProperty = serializedObject.FindProperty("EffectDescription");
        EditorGUILayout.PropertyField(effectDescriptionProperty);

        DrawLine(2);

        DrawSecondaryTitle("Feedbacks");

        SerializedProperty sFXProperty = serializedObject.FindProperty("SfxOnUse");
        EditorGUILayout.PropertyField(sFXProperty);

        SerializedProperty animationConditionProperty = serializedObject.FindProperty("PlayAnimation");
        EditorGUILayout.PropertyField(animationConditionProperty);
        if (animationConditionProperty.boolValue)
        {
            SerializedProperty animationProperty = serializedObject.FindProperty("Animation");
            EditorGUILayout.PropertyField(animationProperty);
        }

        DrawLine(10);

        DrawPrimaryTitle("Surveys");

        SerializedProperty surveyProperty = serializedObject.FindProperty("CustomSurveys");
        EditorGUILayout.PropertyField(surveyProperty);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    public void DrawLine(int size = 1)
    {
        EditorGUILayout.Space();

        Rect rect = EditorGUILayout.GetControlRect(false, size);
        rect.height = size;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

        EditorGUILayout.Space();
    }

    public void DrawPrimaryTitle(string titleText)
    {
        if (s_primaryTitleFontStyle != null)
        {
            GUILayout.Label(titleText, s_primaryTitleFontStyle);
        }
    }

    public void DrawSecondaryTitle(string titleText)
    {
        if (s_secondaryTitleFontStyle != null)
        {
            GUILayout.Label(titleText, s_secondaryTitleFontStyle);
        }
    }
}