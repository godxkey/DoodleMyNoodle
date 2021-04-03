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
    private static Dictionary<Type,Type> s_gameActionSettingAuthTypes;

    private static GUIStyle s_PrimaryTitleFontStyle = null;
    private static GUIStyle s_SecondaryTitleFontStyle = null;

    private void OnEnable()
    {
        if (s_availableTypes == null)
        {
            s_availableTypes = TypeUtility.GetTypesDerivedFrom(typeof(GameAction)).ToArray();
            s_availableTypeNames = s_availableTypes.Select(t => t.Name).ToArray();
        }

        if (s_PrimaryTitleFontStyle == null)
        {
            s_PrimaryTitleFontStyle = new GUIStyle();
            s_PrimaryTitleFontStyle.normal.textColor = Color.white;
            s_PrimaryTitleFontStyle.fontSize = 20;
            s_PrimaryTitleFontStyle.fontStyle = FontStyle.Bold;
        }

        if (s_SecondaryTitleFontStyle == null)
        {
            s_SecondaryTitleFontStyle = new GUIStyle();
            s_SecondaryTitleFontStyle.normal.textColor = Color.white;
            s_SecondaryTitleFontStyle.fontSize = 15;
            s_SecondaryTitleFontStyle.fontStyle = FontStyle.Bold;
        }

        if (s_gameActionSettingAuthTypes == null)
        {
            s_gameActionSettingAuthTypes = new Dictionary<Type, Type>();
        }

        Type[] GameActionSettingAuthTypes = TypeUtility.GetTypesWithAttribute(typeof(GameActionSettingAuthAttribute)).ToArray();
        foreach (Type GameActionSettingAuthType in GameActionSettingAuthTypes)
        {
            GameActionSettingAuthAttribute attribute = (GameActionSettingAuthAttribute)GameActionSettingAuthType.GetCustomAttribute(typeof(GameActionSettingAuthAttribute));
            if (!s_gameActionSettingAuthTypes.ContainsKey(attribute.SettingType))
            {
                s_gameActionSettingAuthTypes.Add(attribute.SettingType, GameActionSettingAuthType);
            }
        }
    }

    private int _listSize = 0;

    public override void OnInspectorGUI()
    {
        var CastedTarget = (GameActionAuth)target;

        EditorGUI.BeginChangeCheck();

        DrawLine(10);

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
                SerializedProperty Values = serializedObject.FindProperty("GameActionSettings");
                Type[] GameActionAuthSettingTypes = selectedGameAction.GetRequiredSettingTypes().Select((simType)=> 
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
                for (int i = Values.arraySize - 1; i >= 0; i--)
                {
                    if (!GameActionAuthSettingTypes.Contains(CastedTarget.GameActionSettings[i].GetType()))
                    {
                        Values.DeleteArrayElementAtIndex(i);
                        CastedTarget.GameActionSettings.RemoveAt(i);
                    }
                }

                // Add missing setting to property list
                for (int i = 0; i < GameActionAuthSettingTypes.Length; i++)
                {
                    Type currentAuthSettingType = GameActionAuthSettingTypes[i];
                    if (currentAuthSettingType == null)
                        continue;

                    if (!CastedTarget.GameActionSettings.Any((x) => x.GetType() == currentAuthSettingType))
                    {
                        Values.InsertArrayElementAtIndex(Values.arraySize);
                        SerializedProperty currentGameActionSetting = Values.GetArrayElementAtIndex(Values.arraySize - 1);
                        var newAuthSetting = Activator.CreateInstance(currentAuthSettingType);
                        currentGameActionSetting.managedReferenceValue = newAuthSetting;
                        CastedTarget.GameActionSettings.Add((GameActionSettingAuthBase)newAuthSetting);
                    }
                }

                EditorGUILayout.Space();
                for (int i = 0; i < Values.arraySize; i++)
                {
                    EditorGUILayout.LabelField(CastedTarget.GameActionSettings[i].GetType().ToString(), EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(Values.GetArrayElementAtIndex(i));
                    EditorGUILayout.Space();
                }
            }
        }

        DrawLine(10);

        DrawPrimaryTitle("Presentation");

        DrawLine(2);

        DrawSecondaryTitle("Description");

        SerializedProperty IconProperty = serializedObject.FindProperty("Icon");
        EditorGUILayout.PropertyField(IconProperty);

        SerializedProperty NameProperty = serializedObject.FindProperty("Name");
        EditorGUILayout.PropertyField(NameProperty);

        SerializedProperty EffectDescriptionProperty = serializedObject.FindProperty("EffectDescription");
        EditorGUILayout.PropertyField(EffectDescriptionProperty);

        DrawLine(2);

        DrawSecondaryTitle("Feedbacks");

        SerializedProperty SFXProperty = serializedObject.FindProperty("SfxOnUse");
        EditorGUILayout.PropertyField(SFXProperty);

        SerializedProperty animationConditionProperty = serializedObject.FindProperty("PlayAnimation");
        EditorGUILayout.PropertyField(animationConditionProperty);
        if (animationConditionProperty.boolValue)
        {
            SerializedProperty animationProperty = serializedObject.FindProperty("Animation");
            EditorGUILayout.PropertyField(animationProperty);
        }

        DrawLine(10);

        DrawPrimaryTitle("Surveys");

        SerializedProperty SurveyrProperty = serializedObject.FindProperty("CustomSurveys");
        EditorGUILayout.PropertyField(SurveyrProperty);

        DrawLine(10);

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
        GUILayout.Label(titleText, s_PrimaryTitleFontStyle);
    }

    public void DrawSecondaryTitle(string titleText)
    {

        GUILayout.Label(titleText, s_SecondaryTitleFontStyle);
    }
}