
public class GameActionParameter
{
    public class Description : GameAction.ParameterDescription { }

    [NetSerializable]
    public class Data : GameAction.ParameterData
    {
        public Data() { }
    }
}
