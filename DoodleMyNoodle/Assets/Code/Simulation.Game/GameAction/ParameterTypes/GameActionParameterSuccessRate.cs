
public class GameActionParameterSuccessRate
{
    public class Description : Action.ParameterDescription
    {
        public Description() { }

        public override Action.ParameterDescriptionType GetParameterDescriptionType()
        {
            return Action.ParameterDescriptionType.SuccessRating;
        }
    }

    [NetSerializable]
    public class Data : Action.ParameterData
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