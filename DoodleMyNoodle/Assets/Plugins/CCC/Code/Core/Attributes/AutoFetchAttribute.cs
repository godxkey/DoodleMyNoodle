using System;
using UnityEngine;

namespace CCC.InspectorDisplay
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AutoFetchAttribute : PropertyAttribute
    {
    }
}