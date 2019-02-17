using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlexTestScript : MonoBehaviour {

    public int tileID = 1;

    void Update()
    {
        transform.position = TestGridService.Instance.grid.GetTilePosition(tileID);

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = CameraService.Instance.ActiveCamera.ScreenToWorldPoint(Input.mousePosition);
            int newTileID = GridTools.FindTileClosestToPosition(TestGridService.Instance.grid, new Vector2(pos.x, pos.y));
            Debug.Log(newTileID);
            tileID = newTileID;
        }
            
    }
}
