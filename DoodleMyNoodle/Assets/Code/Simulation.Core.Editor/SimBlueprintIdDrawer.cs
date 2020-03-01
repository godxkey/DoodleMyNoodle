using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SimBlueprintId))]
public class SimBlueprintIdDrawer : PropertyDrawer
{
    AutoResetDirtyValue<SimBlueprintId> _blueprintId;
    Object _oldRef;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 3 * 18; // 3 lines of 18px
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty typeProperty = property.FindPropertyRelative(nameof(SimBlueprintId.Type));
        SerializedProperty valueProperty = property.FindPropertyRelative(nameof(SimBlueprintId.Value));
        _blueprintId.Set(new SimBlueprintId((SimBlueprintId.BlueprintType)typeProperty.intValue, valueProperty.stringValue));
        if (_blueprintId.IsDirty)
        {
            _oldRef = GetGameObjectReference(_blueprintId.Get());
        }

        EditorGUI.BeginProperty(position, label, property);

        Rect rect = new Rect(position.x, position.y, position.width, 18);

        Object newRef = EditorGUI.ObjectField(rect, label, _oldRef, typeof(GameObject), allowSceneObjects: false);
        if (ReferenceEquals(_oldRef, newRef) == false) // change!
        {
            var newBlueprintId = GetBlueprintFromGameObject(newRef as GameObject);

            typeProperty.enumValueIndex = (int)newBlueprintId.Type;
            valueProperty.stringValue = newBlueprintId.Value;
        }

        EditorGUI.indentLevel++;

        rect.position += Vector2.up * 18;

        GUI.enabled = false;

        EditorGUI.PropertyField(rect, typeProperty, new GUIContent("Type"), true);

        rect.position += Vector2.up * 18;

        EditorGUI.PropertyField(rect, valueProperty, new GUIContent("Value"), true);

        GUI.enabled = true;

        EditorGUI.indentLevel--;


        EditorGUI.EndProperty();
    }

    GameObject GetGameObjectReference(SimBlueprintId blueprintId)
    {
        switch (blueprintId.Type)
        {
            default:
            case SimBlueprintId.BlueprintType.Invalid:
            {
                return null;
            }

            case SimBlueprintId.BlueprintType.Prefab:
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(blueprintId.Value);
                return AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            }

            case SimBlueprintId.BlueprintType.SceneGameObject:
            {
                return null;
            }
        }
    }

    SimBlueprintId GetBlueprintFromGameObject(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return SimBlueprintId.Invalid;
        }

        if (gameObject.GetComponent<SimEntity>() == null)
        {
            Debug.LogWarning("Cannot set blueprint because gameobject doesn't have SimEntity component");
            return SimBlueprintId.Invalid;
        }

        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(gameObject, out string guid, out long localId))
        {
            return SimBlueprintProviderPrefab.MakeBlueprintId(prefabGuid: guid);
        }

        return SimBlueprintId.Invalid;
    }
}
