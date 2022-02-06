
public class GameActionParameterVector
{
    public class Description : Action.ParameterDescription
    {
        public fix SpeedMin = 0;
        public fix SpeedMax = 99;
        public bool UsePreviousParameterOriginLocation = false;

        public Description() { }

        public override Action.ParameterDescriptionType GetParameterDescriptionType()
        {
            return Action.ParameterDescriptionType.Vector;
        }
    }

    [NetSerializable]
    public class Data : Action.ParameterData
    {
        public fix2 Vector;

        public Data() { }

        public Data(fix2 v)
        {
            this.Vector = v;
        }

        public override string ToString()
        {
            return $"{Vector}";
        }
    }
}
