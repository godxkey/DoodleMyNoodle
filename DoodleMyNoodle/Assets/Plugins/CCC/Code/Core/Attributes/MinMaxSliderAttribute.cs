using System;
using UnityEngine;

namespace CCC.InspectorDisplay
{
    public class MinMaxSliderAttribute : PropertyAttribute
    {
        public readonly float Max;
        public readonly float Min;

        public MinMaxSliderAttribute(float min, float max)
        {
            this.Min = min;
            this.Max = max;
        }
    }
}