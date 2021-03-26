
public class GameActionParameterVector
{
    public class Description : GameAction.ParameterDescription
    {
        public fix StrengthMin = 0;
        public fix StrengthMax = 99;

        public Description() { }

        public override GameAction.ParameterDescriptionType GetParameterDescriptionType()
        {
            return GameAction.ParameterDescriptionType.Vector;
        }
    }

    [NetSerializable]
    public class Data : GameAction.ParameterData
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