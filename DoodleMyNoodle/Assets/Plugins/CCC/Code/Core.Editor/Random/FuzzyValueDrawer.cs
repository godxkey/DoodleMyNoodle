using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FuzzyValue))]
public class FuzzyValueDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 4
            + EditorGUIUtility.standardVerticalSpacing * 3;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var propPlateau = property.FindPropertyRelative(nameof(FuzzyValue.DistributionPlateau));
        var propVariation = property.FindPropertyRelative(nameof(FuzzyValue.Variation));
        var propBaseValue = property.FindPropertyRelative(nameof(FuzzyValue.BaseValue));

        AnimationCurve curve = new AnimationCurve();
        var kf0 = new Keyframe(propBaseValue.floatValue - propVariation.floatValue, 0);
        var kf1 = new Keyframe(propBaseValue.floatValue - propVariation.floatValue * propPlateau.floatValue, 1);
        var kf2 = new Keyframe(propBaseValue.floatValue + propVariation.floatValue * propPlateau.floatValue, 1);
        var kf3 = new Keyframe(propBaseValue.floatValue + propVariation.floatValue, 0);

        if (kf0.time != kf1.time)
            curve.AddKey(kf0);
        curve.AddKey(kf1);
        curve.AddKey(kf2);
        if (kf2.time != kf3.time)
            curve.AddKey(kf3);

        for (int i = 0; i < curve.length; i++)
        {
            AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
            AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
        }

        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.CurveField(position, label, curve);

        position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

        using (new EditorGUI.IndentLevelScope())
        {
            EditorGUI.PropertyField(position, propBaseValue);

            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, propVariation, new GUIContent("Variation (+/-)"));
            
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
            using (new EditorGUI.PropertyScope(position, GUIContent.none, propPlateau))
            {
                propPlateau.floatValue = EditorGUI.Slider(position, new GUIContent("Distribution"), propPlateau.floatValue, 0, 1);
            }

            //const float SPACING = 2;
            //const float PLATEAU_RELATIVE_WIDTH = 0.3f;
            //Rect lineRect = position;
            //position.width = (lineRect.width * (1 - PLATEAU_RELATIVE_WIDTH)) - SPACING;
            //EditorGUI.PropertyField(position, propVariation, new GUIContent("Variation (+/-)"));

            //position.x = lineRect.x + position.width + SPACING;

            //position.width = lineRect.width * PLATEAU_RELATIVE_WIDTH;
            //using (new EditorGUI.PropertyScope(position, GUIContent.none, propPlateau))
            //{
            //    propPlateau.floatValue = GUI.HorizontalSlider(position, propPlateau.floatValue, 0, 1);
            //}
        }
    }
}