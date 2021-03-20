
public class GameActionParameterVector
{
    public class Description : GameAction.ParameterDescription
    {
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

        public Data(fix2 Direction)
        {
            this.Vector = Direction;
        }
    }
}