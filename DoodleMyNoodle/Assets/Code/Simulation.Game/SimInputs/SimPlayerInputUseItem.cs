using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public class SimPlayerInputUseItem : SimPlayerInput
{
    public int ItemIndex;
    public GameAction.UseParameters UseData;

    public SimPlayerInputUseItem() {}

    public SimPlayerInputUseItem(int itemIndex, GameAction.UseParameters useData)
    {
        ItemIndex = itemIndex;
        UseData = useData;
    }

    public override string ToString()
    {
        return $"SimPlayerInputUseItem(player:{SimPlayerId.Value}, ItemIndex: {ItemIndex}, ParamCount:{UseData?.ParameterDatas?.Length})";
    }
}
