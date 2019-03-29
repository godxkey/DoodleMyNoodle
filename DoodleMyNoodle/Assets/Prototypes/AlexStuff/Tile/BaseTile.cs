using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTile : MonoBehaviour
{
    private int tileID;
    public int TileID { get { return tileID; } }

    public void SetTileID(int tileID)
    {
        this.tileID = tileID;
    }

    public virtual void OnStepOnTile(GameObject newObjectOnTile)
    {

    }

    public virtual bool CanStepOnTile()
    {
        return true;
    }

    public virtual Color GetTileEditorColor()
    {
        return Color.black;
    }

    public virtual string GetTileEditorName()
    {
        return "DefaultTile";
    }
}
