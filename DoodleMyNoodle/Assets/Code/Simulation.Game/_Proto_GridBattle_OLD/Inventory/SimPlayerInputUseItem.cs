using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public class SimPlayerInputUseItem : SimPlayerInput
{
    public int ItemIndex;
    public GameAction.UseData UseData;

    public SimPlayerInputUseItem() {}

    public SimPlayerInputUseItem(int itemIndex, GameAction.UseData useData)
    {
        ItemIndex = itemIndex;
        UseData = useData;
    }
}
