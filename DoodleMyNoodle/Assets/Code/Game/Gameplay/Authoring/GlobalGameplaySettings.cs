using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Global Gameplay Settings")]
public class GlobalGameplaySettings : ScriptableObject
{
    [Header("Mob References")]
    public GameObject MobSEArcher;
    public GameObject MobSEBird;
    public GameObject MobSEBrute;
    public GameObject MobSEBruteBoss;
    public GameObject MobSEChicken;
    public GameObject MobSEFrog;

    [Header("Economy")]
    public int ShopRerollPrice = 4;
    //public int BaseGoldPerLevel = 5;
    //public int MaxGoalInterest = 5;

#if UNITY_EDITOR
    public static GlobalGameplaySettings GetInstance_EditorOnly()
    {
        return UnityEditor.AssetDatabase.LoadAssetAtPath<GlobalGameplaySettings>(UnityEditor.AssetDatabase.GUIDToAssetPath("924ba638bb9fd114e9283b3951ddb59a"));
    }
#endif
}