using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// fbessette: this structure is intentionally very simple. We'll rework it or extend it as necessary in the future.

/// <summary>
/// Represents a game level
/// </summary>
[CreateAssetMenu(menuName = "DoodleMyNoodle/Level")]
public class Level : ScriptableObject
{
    public SceneInfo[] SimulationScenes;
    public SceneInfo[] PresentationScenes;
}
