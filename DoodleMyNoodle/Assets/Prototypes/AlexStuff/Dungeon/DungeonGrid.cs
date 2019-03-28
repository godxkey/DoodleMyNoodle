using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGrid : MonoBehaviour
{
    [SerializeField]
    private DungeonRoom room;

    [SerializeField]
    private GridData data; // manually put data
    public GridData Data { get { return data; } }

    // Corner Tool
    [SerializeField]
    private Location cornerA;
    public Location CornerA { get { return cornerA; } }
    [SerializeField]
    private Location cornerB;
    public Location CornerB { get { return cornerB; } }

    // Init function
    void Start()
    {
        // Be sure to have a GridData
        VerifyDataValues();

        // Setup the data inside the grid
        Grid grid = new Grid(); // creating grid
        data.SetGridValues(ref grid); // setup grid with our data

        for (int i = 0; i < grid.tileAmount; i++)
        {
            Debug.DrawLine(grid.GetTilePosition(i), new Vector3(grid.GetTilePosition(i).x + 1, grid.GetTilePosition(i).y));
        }
        

        room.SetGrid(grid);
    }

    // Data from the service is normally put into the data
    void VerifyDataValues()
    {
        // Setup Default Values if we don't have data
        if (data == null)
        {
            // we create a data with defaults values
            data = new GridData();
        }
        else
        {
            data.SetData(cornerA, cornerB);
        }
    }

    // Debug Display before hitting Play

    [Space, Header("Debug Gizmo Display")]
    public bool usingTileInfo = false;

    void OnDrawGizmos()
    {
        // Be sure service data is send into the GridData
        VerifyDataValues();

        // There is 2 types of Grid Generation
        if (usingTileInfo)
        {
            // A.B. TODO
        }
        else
        {
            List<Vector3> tilesPos = new List<Vector3>();
            GridBuilder.GetAllPositions(ref tilesPos, data, cornerA.transform.position, cornerB.transform.position);

            for (int i = 0; i < tilesPos.Count; i++)
            {
                Gizmos.color = Color.blue;
                float sizeX = ((Mathf.Abs(cornerA.transform.position.x) + Mathf.Abs(cornerB.transform.position.x)) / data.gridSize);
                float sizeY = ((Mathf.Abs(cornerA.transform.position.y) + Mathf.Abs(cornerB.transform.position.y)) / data.gridSize);
                Gizmos.DrawWireCube(tilesPos[i], new Vector3(sizeX, sizeY, 1));
            }
        }
    }
}
