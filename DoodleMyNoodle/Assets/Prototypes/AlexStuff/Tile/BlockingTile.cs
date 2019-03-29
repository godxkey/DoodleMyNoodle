using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingTile : BaseTile
{
    public override bool CanStepOnTile()
    {
        return false;
    }

    public override Color GetTileEditorColor()
    {
        return Color.red;
    }

    public override string GetTileEditorName()
    {
        return "X";
    }
}
