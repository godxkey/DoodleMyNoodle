using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/GridData")]
public class GridData : ScriptableObject
{
    public bool usingTileData;

    // General Info
    [Header("GENERAL INFO")]
    public int height = 0;

    // Grid Info
    [HideIf("usingTileData"),Header("GRID INFO"), Space]
    public int gridSize = 16; // Amount of Tile
    [HideInInspector]
    public Vector3 cornerALocation;
    [HideInInspector]
    public Vector3 cornerBLocation;

    // Tile Info
    [HideIf("usingTileData"), Header("TILE INFO"), Space]


    // Local Info
    float deltaX;
    float deltaY;

    // Default Constructor for when you didn't link a GridData
    public GridData()
    {
        // Get All Data
        usingTileData = false;
        gridSize = 16;
        cornerALocation = new Vector3(-10, 5, 0);
        cornerBLocation = new Vector3(10, -5, 0);
    }

    // Fill with data when you link a GridData
    public void SetData(Location cornerA, Location cornerB)
    {
        cornerALocation = cornerA.pos;
        cornerBLocation = cornerB.pos;
    }

    // Ask the grid builder to build everything together, and put it into the grid
    public void SetGridValues(ref GridAlex grid)
    {
        // Using gridbuilder to setup/build the grid
        GridBuilder.SetupGrid(this, ref grid, usingTileData);
    }

    // SetGridValues is a one timer, this force it
    private void ForceSetGridValues()
    {
        //Grid.hasBeenSetup = false;
        //GridBuilder.SetupGrid(this, usingTileData);
    }

    // If you modify something in the editor update the system
    void OnValidate()
    {
        if (Application.isPlaying)
            ForceSetGridValues();
    }
}
