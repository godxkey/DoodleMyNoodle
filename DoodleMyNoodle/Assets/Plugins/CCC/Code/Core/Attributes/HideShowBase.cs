using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCC.InspectorDisplay
{
    public abstract class HideShowBaseAttribute : PropertyAttribute
    {
        public readonly string ConditionalMemberName;
        public readonly bool IndentProperty;

        public HideShowBaseAttribute(string conditionalMemberName, bool indent)
        {
            ConditionalMemberName = conditionalMemberName;
            IndentProperty = indent;
        }
    }
}