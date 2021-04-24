using UnityEngine;
using UnityEditor;
using System;

namespace CCC.InspectorDisplay
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    class MinMaxSliderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType != SerializedPropertyType.Vector2 && property.propertyType != SerializedPropertyType.Vector2Int)
            {
                EditorGUI.LabelField(position, label + " / Use only with Vector2");
                EditorGUI.EndProperty();
                return;
            }

            MinMaxSliderAttribute attr = attribute as MinMaxSliderAttribute;

            const float dataWidth = 40;
            const float spacing = 5;
            Rect leftHandSide = position;
            leftHandSide.max = leftHandSide.max - Vector2.right * (dataWidth * 2 + spacing * 2);
            Rect minRegion = new Rect(leftHandSide.xMax + spacing - 30, leftHandSide.y, dataWidth + 30, leftHandSide.height);
            Rect maxRegion = new Rect(minRegion.xMax + spacing - 30, leftHandSide.y, dataWidth + 30, leftHandSide.height);

            float min = 0;
            float max = 0;

            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                Vector2 range = property.vector2Value;
                min = range.x;
                max = range.y;
            }
            else if (property.propertyType == SerializedPropertyType.Vector2Int)
            {
                Vector2Int range = property.vector2IntValue;
                min = range.x;
                max = range.y;
            }

            EditorGUI.BeginChangeCheck();

            EditorGUI.MinMaxSlider(leftHandSide, label, ref min, ref max, attr.Min, attr.Max);

            if (EditorGUI.EndChangeCheck())
            {
                if (property.propertyType == SerializedPropertyType.Vector2)
                {
                    property.vector2Value = new Vector2(min, max);
                }
                else if (property.propertyType == SerializedPropertyType.Vector2Int)
                {
                    property.vector2IntValue = new Vector2Int(Mathf.RoundToInt(min), Mathf.RoundToInt(max));
                }
            }

            EditorGUI.BeginChangeCheck();

            min = EditorGUI.FloatField(minRegion, GUIContent.none, min);
            max = EditorGUI.FloatField(maxRegion, GUIContent.none, max);
            if (EditorGUI.EndChangeCheck())
            {
                if (property.propertyType == SerializedPropertyType.Vector2)
                {
                    property.vector2Value = new Vector2(min, max);
                }
                else if (property.propertyType == SerializedPropertyType.Vector2Int)
                {
                    property.vector2IntValue = new Vector2Int(Mathf.RoundToInt(min), Mathf.RoundToInt(max));
                }
            }

            EditorGUI.EndProperty();
        }
    }
}