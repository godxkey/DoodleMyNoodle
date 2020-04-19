using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/Items/Information Scriptable Object")]
public class ItemVisualInfo : ScriptableObject
{
    public SimAssetIdAuth ID;
    public Sprite Icon;
    public string Name;
    public string Description;
}
