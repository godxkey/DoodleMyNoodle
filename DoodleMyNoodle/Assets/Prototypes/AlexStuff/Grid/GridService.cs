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

    // On Awake, if GridService is already in the scene, set instance and init
    // or else, Service system is going to create it from a prefab made in the project
    void Awake()
    {
        Instance = this;
        Initialize(null);
    }

    // Init function
    public override void Initialize(Action<ICoreService> onComplete)
    {
        // Be sure to have a GridData
        SetupValues();

        // Setup the data inside the grid
        data.SetGridValues();

        // We're ready ! Awake calls the init so we need to check for null
        if(onComplete != null)
            onComplete(this);
    }

    // Data from the service is normally put into the data
    void SetupValues()
    {
        // Setup Default Values if we don't have data
        if (data == null)
        {
            // we create a data with defaults values
            data = new GridData();
        } else
        {
            data.SetData(cornerA, cornerB);
        }
    }

    // Debug Display before hitting Play

    [Space,Header("Debug Gizmo Display")]
    public bool usingTileInfo = false;

    void OnDrawGizmos()
    {
        // Be sure service data is send into the GridData
        SetupValues();

        // There is 2 types of Grid Generation
        if (usingTileInfo)
        {

        } else
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
