
public class GameActionParameterSelfTarget
{
    public class Description : GameAction.ParameterDescription { }

    [NetSerializable]
    public class Popo : GameAction.ParameterData
    {
        public Popo() { }

        public Popo(int parameterIndex) : base(parameterIndex) { }
    }
}
