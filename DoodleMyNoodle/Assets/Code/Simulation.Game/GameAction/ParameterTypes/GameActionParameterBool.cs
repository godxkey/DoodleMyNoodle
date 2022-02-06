
public class GameActionParameterBool
{
    public class Description : Action.ParameterDescription
    {
        public Description() { }

        public override Action.ParameterDescriptionType GetParameterDescriptionType()
        {
            return Action.ParameterDescriptionType.Bool;
        }
    }

    [NetSerializable]
    public class Data : Action.ParameterData
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