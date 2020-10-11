using CCC.InspectorDisplay;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Game System Bank")]
public class GameSystemBank : ScriptableObject
{
    public List<GameSystem> Prefabs = new List<GameSystem>();
}