using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    //public class BaseSmartBrush : GridBrushBase
    //{
    //    protected Sprite GetCurrentSelectedSprite(GameObject brushTarget, Vector3Int position)
    //    {
    //        UnityEngine.Tilemaps.Tilemap currentTileMap = brushTarget.GetComponent<UnityEngine.Tilemaps.Tilemap>();
    //        Tile currentlySelectedTile = currentTileMap.GetTile(position) as Tile;
    //        return currentlySelectedTile.sprite;
    //    }

    //    protected List<GameObject> GetObjectsInCell(GridLayout grid, Transform parent, Vector3Int position)
    //    {
    //        var results = new List<GameObject>();
    //        var childCount = parent.childCount;
    //        for (var i = 0; i < childCount; i++)
    //        {
    //            var child = parent.GetChild(i);
    //            if (position == grid.WorldToCell(child.position))
    //            {
    //                results.Add(child.gameObject);
    //            }
    //        }
    //        return results;
    //    }
    //}

    //[CustomEditor(typeof(BaseSmartBrush))]
    //public class BaseSmartBrushEditor : GridBrushEditor
    //{
    //    public override GameObject[] validTargets
    //    {
    //        get
    //        {
    //            return FindObjectsOfType<Tilemap>().Select(x => x.gameObject).ToArray();
    //        }
    //    }
    //}
}