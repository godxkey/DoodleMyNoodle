using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridService : MonoCoreService<GridService>
{
    // Initialisation Data

    [SerializeField]
    private GridData data;

    public Location cornerA;
    public Location cornerB;

    private Vector3 cornerALocation = new Vector3(-10, 5, 0);
    private Vector3 cornerBLocation = new Vector3(10, -5, 0);

    // Accessible Grid Tool

    GridTools gridTools;

    public GridTools tools
    {
        get
        {
            if (gridTools == null)
            {
                Debug.LogError("GridTools doesn't exist");
                return null;
            }
            else
            {
                return gridTools;
            }
        }

        set
        {
            return;
        }
    }

    void Awake()
    {
        Instance = this;
    }

    public override void Initialize(Action<ICoreService> onComplete)
    {
        // If neither starting corners exist, we need default values
        if (data == null || cornerA == null || cornerB == null)
        {
            data = new GridData(GridData.defaultGridTileSize);
        }
        else
        {
            cornerALocation = cornerA.transform.position;
            cornerBLocation = cornerB.transform.position;
        }

        // Create Grid and pass it to Tools (the one using it)
        Grid grid = null;
        GridBuilder.BuildTiles(ref grid, data, cornerALocation, cornerBLocation);
        gridTools = new GridTools(grid);

        onComplete(this);
    }

    // Debug

    void OnDrawGizmos()
    {
        // If neither starting corners exist, we need default values
        if (data == null || cornerA == null || cornerB == null)
        {
            data = new GridData(GridData.defaultGridTileSize);
        }
        else
        {
            cornerALocation = cornerA.transform.position;
            cornerBLocation = cornerB.transform.position;
        }

        List<Vector3> tilesPos = new List<Vector3>();
        GridBuilder.GetAllPositions(ref tilesPos, data, cornerALocation, cornerBLocation);

        for (int i = 0; i < tilesPos.Count; i++)
        {
            Gizmos.color = Color.blue;
            float sizeX = ((Mathf.Abs(cornerALocation.x) + Mathf.Abs(cornerBLocation.x)) / data.gridSize);
            float sizeY = ((Mathf.Abs(cornerALocation.y) + Mathf.Abs(cornerBLocation.y)) / data.gridSize);
            Gizmos.DrawWireCube(tilesPos[i], new Vector3(sizeX, sizeY, 1));
        }
    }
}
