using Unity.Mathematics;

public class GameActionParameterMiniGame
{
    public enum SuccessRate
    {
        Failed = 1,
        Almost = 2,
        Good = 3,
        Success = 4,
        Critical = 5
    }

    public class Description : GameAction.ParameterDescription
    {
        public bool NeedDirectionnalForce;
        public bool NeedSuccessRate;
        public bool NeedPosition;

        public Description() { }
    }

    [NetSerializable]
    public class Data : GameAction.ParameterData
    {
        public fix3 DirectionnalForce;
        public SuccessRate SuccessRate;
        public int2 Position;

        public Data() { }

        public Data(byte parameterIndex, fix3 DirectionnalForce, SuccessRate SuccessRate, int2 Position)
            : base(parameterIndex)
        {
            this.DirectionnalForce = DirectionnalForce;
            this.SuccessRate = SuccessRate;
            this.Position = Position;
        }
    }
}

