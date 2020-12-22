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

    public override string ToString()
    {
        return $"SimPlayerInputUseItem(player:{SimPlayerId.Value}, ObjectPosition: {ObjectPosition}, ParamCount:{UseData?.ParameterDatas?.Length})";
    }
}
