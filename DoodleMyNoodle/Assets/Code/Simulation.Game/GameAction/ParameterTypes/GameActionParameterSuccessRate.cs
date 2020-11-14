
public class GameActionParameterSuccessRate
{
    public class Description : GameAction.ParameterDescription
    {
        public Description() { }

        public override GameAction.ParameterDescriptionType GetParameterDescriptionType()
        {
            return GameAction.ParameterDescriptionType.SuccessRate;
        }
    }

    [NetSerializable]
    public class Data : GameAction.ParameterData
    {
        public MiniGameSuccessRate SuccessRate;

        public Data() { }

        public Data(MiniGameSuccessRate SuccessRate)
        {
            this.SuccessRate = SuccessRate;
        }
    }
}