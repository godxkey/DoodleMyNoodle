using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public class SimPlayerInputUseItem : SimPlayerInput
{
    public int ItemIndex;
    public Action.UseParameters UseData;

    public SimPlayerInputUseItem() {}

    public SimPlayerInputUseItem(int itemIndex, Action.UseParameters useData)
    {
        ItemIndex = itemIndex;
        UseData = useData;
    }

    public SimPlayerInputUseItem(int itemIndex, Action.ParameterData[] useData)
    {
        ItemIndex = itemIndex;
        UseData = Action.UseParameters.Create(useData);
    }

    public SimPlayerInputUseItem(int itemIndex, List<Action.ParameterData> useData)
    {
        ItemIndex = itemIndex;
        UseData = Action.UseParameters.Create(useData.ToArray());
    }

    public override string ToString()
    {
        return $"SimPlayerInputUseItem(player:{SimPlayerId.Value}, ItemIndex: {ItemIndex}, ParamCount:{UseData?.ParameterDatas?.Length})";
    }
}