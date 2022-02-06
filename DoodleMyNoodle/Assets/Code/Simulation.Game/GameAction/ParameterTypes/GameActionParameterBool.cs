
public class GameActionParameterBool
{
    public class Description : GameAction.ParameterDescription
    {
        public Description() { }

        public override GameAction.ParameterDescriptionType GetParameterDescriptionType()
        {
            return GameAction.ParameterDescriptionType.Bool;
        }
    }

    [NetSerializable]
    public class Data : GameAction.ParameterData
    {
        public bool Value;

        public Data() { }

        public Data(bool v)
        {
            this.Value = v;
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}