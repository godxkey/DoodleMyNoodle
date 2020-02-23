using System;
using UnityEngine;

namespace CCC.InspectorDisplay
{
    public class ReorderableAttribute : PropertyAttribute
    {
        public bool showAdd = true;
        public bool showDelete = true;
        public bool showOrder = true;
        public bool showBox = true;
    }
}