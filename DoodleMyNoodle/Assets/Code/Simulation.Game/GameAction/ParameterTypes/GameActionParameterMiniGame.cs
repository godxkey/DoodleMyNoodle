using Unity.Mathematics;

public class GameActionParameterMiniGame
{
    public class Description : GameAction.ParameterDescription
    {
        public int2 MiniGameLocation;
        public bool NeedDirectionnalForce;
        public bool NeedSuccessRate;
        public bool NeedPosition;

        public Description() { }
    }

    [NetSerializable]
    public class Data : GameAction.ParameterData
    {
        // Put all possible data to extract from minigame
        // Game action take what it needs
        public fix3 DirectionnalForce;
        public MiniGameDescriptionBase.SuccessRate SuccessRate;
        public int2 Position;

        public Data() { }

        public Data(byte parameterIndex, fix3 DirectionnalForce, MiniGameDescriptionBase.SuccessRate SuccessRate, int2 Position)
            : base(parameterIndex)
        {
            this.DirectionnalForce = DirectionnalForce;
            this.SuccessRate = SuccessRate;
            this.Position = Position;
        }

        public Data(byte parameterIndex, MiniGameDescriptionBase.SuccessRate SuccessRate, int2 Position)
            : base(parameterIndex)
        {
            this.SuccessRate = SuccessRate;
            this.Position = Position;
        }

        public Data(byte parameterIndex, fix3 DirectionnalForce, MiniGameDescriptionBase.SuccessRate SuccessRate)
            : base(parameterIndex)
        {
            this.DirectionnalForce = DirectionnalForce;
            this.SuccessRate = SuccessRate;
        }

        public Data(byte parameterIndex, fix3 DirectionnalForce, int2 Position)
            : base(parameterIndex)
        {
            this.DirectionnalForce = DirectionnalForce;
            this.Position = Position;
        }

        public Data(byte parameterIndex, fix3 DirectionnalForce)
            : base(parameterIndex)
        {
            this.DirectionnalForce = DirectionnalForce;
        }

        public Data(byte parameterIndex, MiniGameDescriptionBase.SuccessRate SuccessRate)
            : base(parameterIndex)
        {
            this.SuccessRate = SuccessRate;
        }

        public Data(byte parameterIndex, int2 Position)
            : base(parameterIndex)
        {
            this.Position = Position;
        }
    }
}

