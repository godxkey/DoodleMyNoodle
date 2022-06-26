using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LevelDefinitionMobWaves.MobGroup))]
public class LevelDefinitionMobGroupDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var propDensity = property.FindPropertyRelative(nameof(LevelDefinitionMobWaves.MobGroup.Density));
        var propMob = property.FindPropertyRelative(nameof(LevelDefinitionMobWaves.MobGroup.Mob));
        var propRange = property.FindPropertyRelative(nameof(LevelDefinitionMobWaves.MobGroup.Range));

        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.BeginProperty(position, EditorGUIUtility.TrTempContent("Position"), propRange);
        if (propDensity.enumValueIndex == (int)LevelDefinitionMobWaves.GroupDensity.Single)
        {
            propRange.vector2Value = Vector2.one * GUI.HorizontalSlider(position, propRange.vector2Value.x, 0, 1);
        }
        else
        {
            float min = propRange.vector2Value.x;
            float max = propRange.vector2Value.y;
            EditorGUI.MinMaxSlider(position, ref min, ref max, 0, 1);
            propRange.vector2Value = new Vector2(Mathf.Min(min, max), Mathf.Max(min, max));
        }
        EditorGUI.EndProperty();

        MovePosToNextProperty();

        position.height = EditorGUI.GetPropertyHeight(propDensity);
        EditorGUI.PropertyField(position, propDensity);

        MovePosToNextProperty();

        position.height = EditorGUI.GetPropertyHeight(propMob);
        EditorGUI.PropertyField(position, propMob);

        EditorGUI.EndProperty();

        void MovePosToNextProperty() { position.y += position.height + EditorGUIUtility.standardVerticalSpacing; }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var density = property.FindPropertyRelative(nameof(LevelDefinitionMobWaves.MobGroup.Density));
        var mob = property.FindPropertyRelative(nameof(LevelDefinitionMobWaves.MobGroup.Mob));
        var range = property.FindPropertyRelative(nameof(LevelDefinitionMobWaves.MobGroup.Range));
        return EditorGUI.GetPropertyHeight(density) + EditorGUIUtility.standardVerticalSpacing
            + EditorGUI.GetPropertyHeight(mob) + EditorGUIUtility.standardVerticalSpacing
            + EditorGUI.GetPropertyHeight(range);
    }
}