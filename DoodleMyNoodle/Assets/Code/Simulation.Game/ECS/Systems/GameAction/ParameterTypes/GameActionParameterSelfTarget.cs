
public class GameActionParameterSelfTarget
{
    public class Description : GameAction.ParameterDescription { }

    [NetSerializable]
    public class Data : GameAction.ParameterData
    {
        public Data() { }

        public Data(int parameterIndex) : base(parameterIndex) { }
    }
}
