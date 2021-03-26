
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
        public SurveySuccessRating SuccessRate;

        public Data() { }

        public Data(SurveySuccessRating SuccessRate)
        {
            this.SuccessRate = SuccessRate;
        }

        public override string ToString()
        {
            return $"{SuccessRate}";
        }
    }
}