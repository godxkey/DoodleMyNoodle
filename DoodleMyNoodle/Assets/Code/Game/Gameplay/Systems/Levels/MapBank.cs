using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName ="DoodleMyNoodle/Map Bank")]
public class MapBank : ScriptableObject
{
    [FormerlySerializedAs("Levels")]
    public List<Map> Maps;
}