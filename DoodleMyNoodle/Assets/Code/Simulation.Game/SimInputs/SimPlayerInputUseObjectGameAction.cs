using System.Collections.Generic;
using Unity.Mathematics;

[NetSerializable]
public class SimPlayerInputUseObjectGameAction : SimPlayerInput
{
    public int2 ObjectPosition;
    public GameAction.UseParameters UseData;

    public SimPlayerInputUseObjectGameAction() { }

    public SimPlayerInputUseObjectGameAction(int2 objectPosition, GameAction.UseParameters useData)
    {
        ObjectPosition = objectPosition;
        UseData = useData;
    }

    public SimPlayerInputUseObjectGameAction(int2 objectPosition, GameAction.ParameterData[] useData)
    {
        ObjectPosition = objectPosition;
        UseData = GameAction.UseParameters.Create(useData);
    }

    public SimPlayerInputUseObjectGameAction(int2 objectPosition, List<GameAction.ParameterData> useData)
    {
        ObjectPosition = objectPosition;
        UseData = GameAction.UseParameters.Create(useData.ToArray());
    }

    public override string ToString()
    {
        return $"SimPlayerInputUseItem(player:{SimPlayerId.Value}, ObjectPosition: {ObjectPosition}, ParamCount:{UseData?.ParameterDatas?.Length})";
    }
}
