using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// fbessette: could be nice to automatize that

[CreateAssetMenu(menuName ="DoodleMyNoodle/Level Bank")]
public class LevelBank : ScriptableObject
{
    public List<Level> Levels;
}
