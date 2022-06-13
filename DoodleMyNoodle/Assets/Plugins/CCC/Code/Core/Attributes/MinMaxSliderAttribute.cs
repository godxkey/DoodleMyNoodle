using System;
using UnityEngine;

namespace CCC.InspectorDisplay
{
    public class MinMaxSliderAttribute : PropertyAttribute
    {
        public readonly float Max;
        public readonly float Min;
        public readonly bool DisplayValues;

        public MinMaxSliderAttribute(float min, float max, bool displayValues = true)
        {
            this.Min = min;
            this.Max = max;
            DisplayValues = displayValues;
        }
    }
}