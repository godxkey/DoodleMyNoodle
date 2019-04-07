using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RushFeedbacks : MonoBehaviour
{
    public Transform tileContainer;

    public List<SpriteRenderer> tiles = new List<SpriteRenderer>();

    public GameObject battleFeedback;

    public enum TileType
    {
        Normal,
        Available,
        Disable,
        Hover
    }

    public void ChangeTileDisplay(TileType newDisplayType, Vector2 position)
    {
        switch (newDisplayType)
        {
            case TileType.Normal:
                GetTile(position).color = Color.black;
                break;
            case TileType.Available:
                GetTile(position).color = Color.green;
                break;
            case TileType.Disable:
                GetTile(position).color = Color.red;
                break;
            case TileType.Hover:
                GetTile(position).color = Color.yellow;
                break;
            default:
                break;
        }
    }

    public void SpawnBattleFeedbackOnTile(Vector2 position)
    {
        Instantiate(battleFeedback, tileContainer).GetComponent<Transform>().localPosition = position;
    }

    public void ClearAll()
    {
        foreach (var tile in tiles)
        {
            tile.color = Color.black;
        }
    }

    private SpriteRenderer GetTile(Vector2 position)
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            Vector3 currentTilePos = tiles[i].transform.localPosition;
            if(currentTilePos.x == position.x && currentTilePos.y == position.y)
            {
                return tiles[i];
            }
        }
        return null;
    }
}
