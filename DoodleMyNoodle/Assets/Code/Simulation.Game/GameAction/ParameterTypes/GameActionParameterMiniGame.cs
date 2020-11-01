using Unity.Mathematics;

public class GameActionParameterMiniGame
{
    public class Description : GameAction.ParameterDescription
    {
        public int2 MiniGameLocation;

        public Description() { }

        public Description(int2 MiniGameLocation) 
        {
            this.MiniGameLocation = MiniGameLocation;
        }
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

        public Data(fix3 DirectionnalForce, MiniGameDescriptionBase.SuccessRate SuccessRate, int2 Position)
        {
            this.DirectionnalForce = DirectionnalForce;
            this.SuccessRate = SuccessRate;
            this.Position = Position;
        }

        public Data(MiniGameDescriptionBase.SuccessRate SuccessRate, int2 Position)
        {
            this.SuccessRate = SuccessRate;
            this.Position = Position;
        }

        public Data(fix3 DirectionnalForce, MiniGameDescriptionBase.SuccessRate SuccessRate)
        {
            this.DirectionnalForce = DirectionnalForce;
            this.SuccessRate = SuccessRate;
        }

        public Data(fix3 DirectionnalForce, int2 Position)
        {
            this.DirectionnalForce = DirectionnalForce;
            this.Position = Position;
        }

        public Data(fix3 DirectionnalForce)
        {
            this.DirectionnalForce = DirectionnalForce;
        }

        public Data(MiniGameDescriptionBase.SuccessRate SuccessRate)
        {
            this.SuccessRate = SuccessRate;
        }

        public Data(int2 Position)
        {
            this.Position = Position;
        }
    }
}

