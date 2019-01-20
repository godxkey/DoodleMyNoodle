using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlexTestScript : MonoBehaviour {

    public int tileID = 1;

    void Update()
    {
        transform.position = Grid.GetTilePosition(tileID);
    }
}
