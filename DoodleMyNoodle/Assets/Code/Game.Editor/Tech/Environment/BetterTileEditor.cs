using UnityEditor;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.Tilemaps;
using UTileFlags = UnityEngine.Tilemaps.TileFlags;

[CustomEditor(typeof(Tile))]
public class BetterTileEditor : Editor
{
    private const float k_PreviewWidth = 32;
    private const float k_PreviewHeight = 32;

    private SerializedProperty m_Color;
    private SerializedProperty m_ColliderType;
    private SerializedProperty m_Sprite;
    private SerializedProperty m_Flags;
    private SerializedProperty m_Transform;

    private Tile tile
    {
        get { return (target as Tile); }
    }

    private static class Styles
    {
        public static readonly GUIContent invalidMatrixLabel = EditorGUIUtility.TrTextContent("Invalid Matrix", "No valid Position / Rotation / Scale components available for this matrix");
        public static readonly GUIContent resetMatrixLabel = EditorGUIUtility.TrTextContent("Reset Matrix");
        public static readonly GUIContent previewLabel = EditorGUIUtility.TrTextContent("Preview", "Preview of tile with attributes set");

        public static readonly GUIContent spriteEditorLabel = EditorGUIUtility.TrTextContent("Sprite Editor");
        public static readonly GUIContent offsetLabel = EditorGUIUtility.TrTextContent("Offset");
        public static readonly GUIContent rotationLabel = EditorGUIUtility.TrTextContent("Rotation");
        public static readonly GUIContent scaleLabel = EditorGUIUtility.TrTextContent("Scale");
    }

    public void OnEnable()
    {
        m_Color = serializedObject.FindProperty("m_Color");
        m_ColliderType = serializedObject.FindProperty("m_ColliderType");
        m_Sprite = serializedObject.FindProperty("m_Sprite");
        m_Flags = serializedObject.FindProperty("m_Flags");
        m_Transform = serializedObject.FindProperty("m_Transform");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        bool customTransform = (((UTileFlags)m_Flags.intValue) & UTileFlags.LockTransform) != 0;
        customTransform = EditorGUILayout.Toggle("Custom Transform", customTransform);
        m_Flags.intValue = customTransform ? m_Flags.intValue | (int)UTileFlags.LockTransform : m_Flags.intValue & (int)~UTileFlags.LockTransform;

        if (customTransform)
        {
            Tile mainTile = target as Tile;
            Matrix4x4 transform = mainTile.transform;
            float rotation = transform.rotation.eulerAngles.z;
            bool flipX = transform.lossyScale.x == -1;
            bool flipY = transform.lossyScale.y == -1;

            rotation = EditorGUILayout.FloatField("Rotation", rotation);
            flipX = EditorGUILayout.Toggle("Mirror X", flipX);
            flipY = EditorGUILayout.Toggle("Mirror Y", flipY);

            transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, rotation), new Vector3(flipX ? -1 : 1, flipY ? -1 : 1, 1));

            foreach (Tile tile in targets)
            {
                tile.transform = transform;
                EditorUtility.SetDirty(tile);
            }
        }
        else
        {
            foreach (Tile tile in targets)
            {
                tile.transform = Matrix4x4.identity;
            }
        }

        EditorGUILayout.PropertyField(m_Sprite);
        EditorGUILayout.PropertyField(m_Color);
        EditorGUILayout.PropertyField(m_ColliderType);

        serializedObject.ApplyModifiedProperties();
    }

    public static Matrix4x4 TransformMatrixOnGUI(Matrix4x4 matrix)
    {
        Matrix4x4 val = matrix;
        if (matrix.ValidTRS())
        {
            EditorGUI.BeginChangeCheck();

            Vector3 pos = Round(matrix.GetColumn(3), 3);
            Vector3 euler = Round(matrix.rotation.eulerAngles, 3);
            Vector3 scale = Round(matrix.lossyScale, 3);
            pos = EditorGUILayout.Vector3Field(Styles.offsetLabel, pos);
            euler = EditorGUILayout.Vector3Field(Styles.rotationLabel, euler);
            scale = EditorGUILayout.Vector3Field(Styles.scaleLabel, scale);

            if (EditorGUI.EndChangeCheck() && scale.x != 0f && scale.y != 0f && scale.z != 0f)
            {
                val = Matrix4x4.TRS(pos, Quaternion.Euler(euler), scale);
            }
        }
        else
        {
            GUILayout.BeginVertical();
            GUILayout.Label(Styles.invalidMatrixLabel);
            if (GUILayout.Button(Styles.resetMatrixLabel))
            {
                val = Matrix4x4.identity;
            }
            GUILayout.EndVertical();
        }
        return val;
    }

    private static Vector3 Round(Vector3 value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return new Vector3(
            Mathf.Round(value.x * mult) / mult,
            Mathf.Round(value.y * mult) / mult,
            Mathf.Round(value.z * mult) / mult
        );
    }

    //public override void OnInspectorGUI()
    //{
    //    base.OnInspectorGUI();

    //    serializedObject.Update();

    //    var tile = target as Tile;
    //    var transform = tile.transform;
    //    var newRotation = EditorGUILayout.Vector3Field("Rotation", transform.rotation.eulerAngles);

    //    transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(newRotation), Vector3.one);
    //    tile.transform = transform;

    //    serializedObject.ApplyModifiedProperties();
    //}
}
