using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTools
{
    public static int FindTileClosestToPosition(Grid grid, Vector2 pos)
    {
        int currentTile = Mathf.RoundToInt(grid.tileAmount / 2);
        Vector3 currentPos = grid.GetTilePosition(currentTile);

        int previousTile = grid.tileAmount; // at start we're checking every tile

        while (true)
        {
            // is it this tile ?
            if (IsPosWithinTile(currentPos, grid.tileSizeX, grid.tileSizeY, pos))
            {
                break;
            }
            else
            {
                int xSector;
                int ySector;

                if (IsXWithinTile(currentPos, grid.tileSizeX, grid.tileSizeY, pos.x))
                {
                    xSector = 0;
                }
                else
                {
                    if (pos.x < (currentPos.x - (grid.tileSizeX / 2)))
                    {
                        xSector = -1;
                    }
                    else
                    {
                        xSector = 1;
                    }
                }

                if (IsYWithinTile(currentPos, grid.tileSizeX, grid.tileSizeY, pos.y))
                {
                    ySector = 0;
                }
                else
                {
                    if (pos.y < (currentPos.y - (grid.tileSizeY / 2)))
                    {
                        ySector = -1;
                    }
                    else
                    {
                        ySector = 1;
                    }
                }

                int sectionLenght = Mathf.Abs(Mathf.RoundToInt((previousTile - currentTile) / 2));

                if (sectionLenght < 1)
                    break;

                previousTile = currentTile;

                if (DoesSectorMeansUpperSection(xSector, ySector))
                {
                    currentTile = currentTile + sectionLenght;
                    currentPos = grid.GetTilePosition(currentTile);
                }
                else
                {
                    currentTile = currentTile - sectionLenght;
                    currentPos = grid.GetTilePosition(currentTile);
                }
            }
        }

        return currentTile;
    }

    private static bool DoesSectorMeansUpperSection(int x, int y)
    {
        if((x == 0 && y == 1) || (x == -1 && y == 0) || (x == -1 && y == 1) || (x == 1 && y == 1))
        {
            return false;
        } else
        {
            return true;
        }
    }

    private static bool IsPosWithinTile(Vector2 tilePos, float sizeX, float sizeY, Vector2 currentPos)
    {
        return (tilePos.x - (sizeX / 2)) <= currentPos.x && 
               currentPos.x <= (tilePos.x + (sizeX / 2)) && 
               (tilePos.y - (sizeY / 2)) <= currentPos.y && 
               currentPos.y <= (tilePos.y + (sizeY / 2));
    }

    private static bool IsXWithinTile(Vector2 tilePos, float sizeX, float sizeY, float x)
    {
        return (tilePos.x - (sizeX / 2)) <= x &&
               x <= (tilePos.x + (sizeX / 2));
    }

    private static bool IsYWithinTile(Vector2 tilePos, float sizeX, float sizeY, float y)
    {
        return (tilePos.y - (sizeY / 2)) <= y &&
               y <= (tilePos.y + (sizeY / 2));
    }

    public static List<Vector3> FindPath(Grid grid, int startTileID, int endTileID)
    {
        return null;
    }

    public static List<Vector3> GetCornerPos()
    {
        return null;
    }

    public static List<Vector3> GetNextTileInDirection(int tileID, Vector2 direction)
    {
        return null;
    }

    public static List<Vector3> GetAllTileAround(int tileID, int tileRange)
    {
        return null;
    }
}
