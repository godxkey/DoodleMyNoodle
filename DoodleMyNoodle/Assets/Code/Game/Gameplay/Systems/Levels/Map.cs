using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Map")]
public class Map : ScriptableObject
{
    public SceneInfo[] SimulationScenes;
    public SceneInfo[] PresentationScenes;
}
