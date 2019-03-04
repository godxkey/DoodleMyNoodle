using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomIdentifier : MonoBehaviour
{
    [SerializeField]
    private DungeonRoom associatedRoom;

    private void OnTriggerEnter(Collider other)
    {
        Player currentPlayer = other.GetComponent<Player>();
        if (currentPlayer != null)
            currentPlayer.SetCurrentDungeonRoom(associatedRoom);
    }
}
