using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Items/Item Bank")]
public class ItemBank : ScriptableObject
{
    public List<GameObject> Items;
}