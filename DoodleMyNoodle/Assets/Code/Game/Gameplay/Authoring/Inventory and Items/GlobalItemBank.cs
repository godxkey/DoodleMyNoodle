using CCC.InspectorDisplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Items/Sim Item Bank")]
public class GlobalItemBank : ScriptableObject
{
    public List<ItemAuth> Items;
}