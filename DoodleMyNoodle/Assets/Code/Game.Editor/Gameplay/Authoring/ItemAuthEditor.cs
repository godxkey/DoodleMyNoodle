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
    private static GUIStyle s_primaryTitleFontStyle = null;
    private static GUIStyle s_secondaryTitleFontStyle = null;

    private SerializedProperty _cooldownProp;
    private SerializedProperty _cooldownDurationProp;
    private SerializedProperty _stackableProp;
    private SerializedProperty _apCostProp;
    private SerializedProperty _iconProp;
    private SerializedProperty _iconTintProp;
    private SerializedProperty _nameProp;
    private SerializedProperty _effectDescriptionProp;
    private SerializedProperty _hideInInventory;
    private SerializedProperty _canBeUsedAtAnytime;

    private void OnEnable()
    {
        _cooldownProp = serializedObject.FindProperty(nameof(ItemAuth.CooldownType));
        _cooldownDurationProp = serializedObject.FindProperty(nameof(ItemAuth.CooldownDuration));
        _stackableProp = serializedObject.FindProperty(nameof(ItemAuth.IsStackable));
        _apCostProp = serializedObject.FindProperty(nameof(ItemAuth.ApCost));
        _iconProp = serializedObject.FindProperty(nameof(ItemAuth.Icon));
        _iconTintProp = serializedObject.FindProperty(nameof(ItemAuth.IconTint));
        _nameProp = serializedObject.FindProperty(nameof(ItemAuth.Name));
        _effectDescriptionProp = serializedObject.FindProperty(nameof(ItemAuth.EffectDescription));
        _hideInInventory = serializedObject.FindProperty(nameof(ItemAuth.HideInInventory));
        _canBeUsedAtAnytime = serializedObject.FindProperty(nameof(ItemAuth.UsableInOthersTurn));

        InitStaticData();
    }

    public override void OnInspectorGUI()
    {
        var castedTarget = (ItemAuth)target;

        EditorGUI.BeginChangeCheck();

        DrawPrimaryTitle("Simulation");

        EditorGUILayout.PropertyField(_apCostProp);
        EditorGUILayout.PropertyField(_cooldownProp);
        if (castedTarget.HasCooldown)
            EditorGUILayout.PropertyField(_cooldownDurationProp);
        EditorGUILayout.PropertyField(_stackableProp);

        DrawLine(10);

        DrawPrimaryTitle("Presentation");

        EditorGUILayout.PropertyField(_iconProp);
        EditorGUILayout.PropertyField(_iconTintProp);
        EditorGUILayout.PropertyField(_nameProp);
        EditorGUILayout.PropertyField(_effectDescriptionProp);
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