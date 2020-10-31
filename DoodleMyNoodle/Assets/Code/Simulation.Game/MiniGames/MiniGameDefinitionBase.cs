using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/MiniGames/Definition/Default")]
public class MiniGameDefinitionBase : ScriptableObject
{
    public string Name;
    public GameObject Prefab;

    public MiniGameDescriptionBase CustomDescription;
}