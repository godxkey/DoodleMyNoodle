using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private DungeonRoom currentRoom;

    [SerializeField]
    private int tileID = 1;

    void Update()
    {
        if(currentRoom != null)
        {
            UpdateMovement();
        }

    }

    private void UpdateMovement()
    {
        transform.position = currentRoom.Grid.GetTilePosition(tileID);

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = CameraService.Instance.ActiveCamera.ScreenToWorldPoint(Input.mousePosition);
            int newTileID = GridTools.FindTileClosestToPosition(currentRoom.Grid, new Vector2(pos.x, pos.y));
            tileID = newTileID;
        }
    }

    public void SetCurrentDungeonRoom(DungeonRoom room)
    {
        currentRoom = room;
    }
}
