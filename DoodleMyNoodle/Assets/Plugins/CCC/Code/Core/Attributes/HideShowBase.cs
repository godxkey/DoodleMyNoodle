using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCC.InspectorDisplay
{
    public abstract class HideShowBaseAttribute : PropertyAttribute
    {
        public readonly string ConditionalMemberName;

        public HideShowBaseAttribute(string conditionalMemberName)
        {
            this.ConditionalMemberName = conditionalMemberName;
        }
    }
}