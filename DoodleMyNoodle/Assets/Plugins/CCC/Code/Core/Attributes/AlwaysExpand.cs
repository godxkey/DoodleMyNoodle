using UnityEngine;

namespace CCC.InspectorDisplay
{
    public class AlwaysExpandAttribute : PropertyAttribute
    {
        public AlwaysExpandAttribute() { }

        public bool UseRootPropertyName = false;
    }
}