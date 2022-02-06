using System.Collections.Generic;
using Unity.Mathematics;

[NetSerializable]
public class SimPlayerInputUseObjectGameAction : SimPlayerInput
{
    public fix2 ObjectPosition;
    public Action.UseParameters UseData;

    public SimPlayerInputUseObjectGameAction() { }

    public SimPlayerInputUseObjectGameAction(fix2 objectPosition, Action.UseParameters useData)
    {
        ObjectPosition = objectPosition;
        UseData = useData;
    }

    public SimPlayerInputUseObjectGameAction(fix2 objectPosition, Action.ParameterData[] useData)
    {
        ObjectPosition = objectPosition;
        UseData = Action.UseParameters.Create(useData);
    }

    public SimPlayerInputUseObjectGameAction(fix2 objectPosition, List<Action.ParameterData> useData)
    {
        ObjectPosition = objectPosition;
        UseData = Action.UseParameters.Create(useData.ToArray());
    }

    public override string ToString()
    {
        return $"SimPlayerInputUseItem(player:{SimPlayerId.Value}, ObjectPosition: {ObjectPosition}, ParamCount:{UseData?.ParameterDatas?.Length})";
    }
}
