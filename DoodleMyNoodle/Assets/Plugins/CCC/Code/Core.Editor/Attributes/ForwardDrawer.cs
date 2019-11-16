using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ForwardAttribute))]
public class ForwardDrawer : PropertyDrawer
{
    GUIContent newLabel = new GUIContent();
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = 0;
        foreach (SerializedProperty child in new DirectChildEnumerator(property))
        {
            newLabel.text = property.displayName;
            totalHeight += EditorGUI.GetPropertyHeight(property, newLabel, true) + 2; // + padding
        }

        return totalHeight - 2;  // we have to subtract 2px of extra padding that unity added because of the last child
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        foreach (SerializedProperty child in new DirectChildEnumerator(property))
        {
            newLabel.text = property.displayName;
            EditorGUI.PropertyField(position, property, newLabel, true);
            position.y += EditorGUI.GetPropertyHeight(property, true);
            position.y += 2; // spacing
        }
    }

    struct DirectChildEnumerator
    {
        SerializedProperty _property;
        bool _enterChildren;
        string _parentPath;
        public DirectChildEnumerator(SerializedProperty property)
        {
            _property = property;
            _enterChildren = property.hasChildren;
            _parentPath = property.propertyPath;
        }
        public DirectChildEnumerator GetEnumerator() => this;

        public SerializedProperty Current => _property;

        public bool MoveNext()
        {
            bool result = _property.Next(_enterChildren);

            _enterChildren = false;

            return result && _property.propertyPath.Contains(_parentPath);
        }
    }
}
