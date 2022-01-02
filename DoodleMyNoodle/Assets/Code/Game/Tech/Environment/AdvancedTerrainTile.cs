using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Terrain Tiles, similar to Pipeline Tiles, are tiles which take into consideration its orthogonal and diagonal neighboring tiles and displays a sprite depending on whether the neighboring tile is the same tile.
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "New Terrain Tile", menuName = "2D/Tiles/DMN/Advanced Terrain Tile", order = 82)]
public class AdvancedTerrainTile : TileBase, ISerializationCallbackReceiver
{
    [SerializeField] private TileBase[] _sisterTiles;

    /// <summary>
    /// The Sprites used for defining the Terrain.
    /// </summary>
    [SerializeField]
    public SpriteData[] SpriteDatas;


    [Serializable]
    public class SpriteData
    {
        public Sprite Sprite;
        public float Rotation;
        public bool FlipX;
        public bool FlipY;
        public Matrix4x4 Transform;

        public void RecalculateTransform()
        {
            Transform = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, Rotation), new Vector3(FlipX ? -1 : 1, FlipY ? -1 : 1, 1));
        }
    }
    
    private HashSet<TileBase> _sisterTilesHashSet = new HashSet<TileBase>();

    /// <summary>
    /// This method is called when the tile is refreshed.
    /// </summary>
    /// <param name="position">Position of the Tile on the Tilemap.</param>
    /// <param name="tilemap">The Tilemap the tile is present on.</param>
    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        for (int yd = -1; yd <= 1; yd++)
            for (int xd = -1; xd <= 1; xd++)
            {
                Vector3Int pos = new Vector3Int(position.x + xd, position.y + yd, position.z);
                if (TileValue(tilemap, pos))
                    tilemap.RefreshTile(pos);
            }
    }

    /// <summary>
    /// Retrieves any tile rendering data from the scripted tile.
    /// </summary>
    /// <param name="position">Position of the Tile on the Tilemap.</param>
    /// <param name="tilemap">The Tilemap the tile is present on.</param>
    /// <param name="tileData">Data to render the tile.</param>
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        UpdateTile(position, tilemap, ref tileData);
    }

    private void UpdateTile(Vector3Int location, ITilemap tileMap, ref TileData tileData)
    {
        tileData.transform = Matrix4x4.identity;
        tileData.color = Color.white;

        int mask = TileValue(tileMap, location + new Vector3Int(0, 1, 0)) ? 1 : 0;
        mask += TileValue(tileMap, location + new Vector3Int(1, 1, 0)) ? 2 : 0;
        mask += TileValue(tileMap, location + new Vector3Int(1, 0, 0)) ? 4 : 0;
        mask += TileValue(tileMap, location + new Vector3Int(1, -1, 0)) ? 8 : 0;
        mask += TileValue(tileMap, location + new Vector3Int(0, -1, 0)) ? 16 : 0;
        mask += TileValue(tileMap, location + new Vector3Int(-1, -1, 0)) ? 32 : 0;
        mask += TileValue(tileMap, location + new Vector3Int(-1, 0, 0)) ? 64 : 0;
        mask += TileValue(tileMap, location + new Vector3Int(-1, 1, 0)) ? 128 : 0;

        byte original = (byte)mask;
        if ((original | 254) < 255) { mask = mask & 125; }
        if ((original | 251) < 255) { mask = mask & 245; }
        if ((original | 239) < 255) { mask = mask & 215; }
        if ((original | 191) < 255) { mask = mask & 95; }

        int index = GetIndex((byte)mask);
        if (index >= 0 && index < SpriteDatas.Length && TileValue(tileMap, location))
        {
            tileData.sprite = SpriteDatas[index].Sprite;
            tileData.transform = GetTransform((byte)mask) * SpriteDatas[index].Transform;
            tileData.color = Color.white;
            tileData.flags = UnityEngine.Tilemaps.TileFlags.LockTransform | UnityEngine.Tilemaps.TileFlags.LockColor;
            tileData.colliderType = Tile.ColliderType.Sprite;
        }
    }

    private bool TileValue(ITilemap tileMap, Vector3Int position)
    {
        TileBase tile = tileMap.GetTile(position);
        return tile != null && (tile == this || _sisterTilesHashSet.Contains(tile));
    }

    private int GetIndex(byte mask)
    {
        switch (mask)
        {
            case 0: return 0;
            case 1:
            case 4:
            case 16:
            case 64: return 1;
            case 5:
            case 20:
            case 80:
            case 65: return 2;
            case 7:
            case 28:
            case 112:
            case 193: return 3;
            case 17:
            case 68: return 4;
            case 21:
            case 84:
            case 81:
            case 69: return 5;
            case 23:
            case 92:
            case 113:
            case 197: return 6;
            case 29:
            case 116:
            case 209:
            case 71: return 7;
            case 31:
            case 124:
            case 241:
            case 199: return 8;
            case 85: return 9;
            case 87:
            case 93:
            case 117:
            case 213: return 10;
            case 95:
            case 125:
            case 245:
            case 215: return 11;
            case 119:
            case 221: return 12;
            case 127:
            case 253:
            case 247:
            case 223: return 13;
            case 255: return 14;
        }
        return -1;
    }

    private Matrix4x4 GetTransform(byte mask)
    {
        switch (mask)
        {
            case 4:
            case 20:
            case 28:
            case 68:
            case 84:
            case 92:
            case 116:
            case 124:
            case 93:
            case 125:
            case 221:
            case 253:
                return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -90f), Vector3.one);
            case 16:
            case 80:
            case 112:
            case 81:
            case 113:
            case 209:
            case 241:
            case 117:
            case 245:
            case 247:
                return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -180f), Vector3.one);
            case 64:
            case 65:
            case 193:
            case 69:
            case 197:
            case 71:
            case 199:
            case 213:
            case 215:
            case 223:
                return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -270f), Vector3.one);
        }
        return Matrix4x4.identity;
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        _sisterTilesHashSet.Clear();
        foreach (var item in _sisterTiles)
        {
            _sisterTilesHashSet.Add(item);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AdvancedTerrainTile))]
public class AdvancedTerrainTileEditor : Editor
{
    private AdvancedTerrainTile Tile { get { return (AdvancedTerrainTile)target; } }

    private SerializedProperty _sisterTiles;

    /// <summary>
    /// OnEnable for TerrainTile.
    /// </summary>
    public void OnEnable()
    {
        _sisterTiles = serializedObject.FindProperty("_sisterTiles");

        if (Tile.SpriteDatas == null || Tile.SpriteDatas.Length != 15)
        {
            Tile.SpriteDatas = new AdvancedTerrainTile.SpriteData[15];
            EditorUtility.SetDirty(Tile);
        }
    }

    /// <summary>
    /// Draws an Inspector for the Terrain Tile.
    /// </summary>
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_sisterTiles);

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("Place sprites shown based on the contents of the sprite.");
        EditorGUILayout.Space();

        float oldLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 210;

        EditorGUI.BeginChangeCheck();
        DrawSpriteSettings(0, "Filled");
        DrawSpriteSettings(1, "Three Sides");
        DrawSpriteSettings(2, "Two Sides and One Corner");
        DrawSpriteSettings(3, "Two Adjacent Sides");
        DrawSpriteSettings(4, "Two Opposite Sides");
        DrawSpriteSettings(5, "One Side and Two Corners");
        DrawSpriteSettings(6, "One Side and One Lower Corner");
        DrawSpriteSettings(7, "One Side and One Upper Corner");
        DrawSpriteSettings(8, "One Side");
        DrawSpriteSettings(9, "Four Corners");
        DrawSpriteSettings(10, "Three Corners");
        DrawSpriteSettings(11, "Two Adjacent Corners");
        DrawSpriteSettings(12, "Two Opposite Corners");
        DrawSpriteSettings(13, "One Corner");
        DrawSpriteSettings(14, "Empty");
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(Tile);

        EditorGUIUtility.labelWidth = oldLabelWidth;
    }

    private void DrawSpriteSettings(int i, string name)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        AdvancedTerrainTile.SpriteData data = Tile.SpriteDatas[i];
        data.Sprite = (Sprite)EditorGUILayout.ObjectField(name, Tile.SpriteDatas[i].Sprite, typeof(Sprite), false, null);
        data.Rotation = EditorGUILayout.FloatField("Rotation", data.Rotation);
        data.FlipX = EditorGUILayout.Toggle("Mirror X", data.FlipX);
        data.FlipY = EditorGUILayout.Toggle("Mirror Y", data.FlipY);
        data.RecalculateTransform();

        EditorGUILayout.EndVertical();
    }
}
#endif