using System.Collections.Generic;
using UnityEngine;

public class ViewBindingDefinitionBank : ScriptableObject
{
    public List<ViewBindingDefinition> ViewBindingDefinitions = new List<ViewBindingDefinition>();
    public List<SimAsset> SimAssets = new List<SimAsset>();
}