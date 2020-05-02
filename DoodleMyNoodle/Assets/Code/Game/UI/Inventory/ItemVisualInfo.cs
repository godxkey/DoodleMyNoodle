using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Items/Item Visuel Info")]
public class ItemVisualInfo : ScriptableObject
{
    public SimAssetIdAuth ID;
    public Sprite Icon;
    public string Name;
    public string Description;
}
