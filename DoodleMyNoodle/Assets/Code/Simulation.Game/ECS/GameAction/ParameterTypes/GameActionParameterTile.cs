using Unity.Mathematics;


public enum TileFilterFlags
{
    Navigable = 1 << 0,
    NonNavigable = 1 << 1,


    Occupied = 1 << 2,
    Inoccupied = 1 << 3,

    NotEmpty = 1 << 4,
}

public class GameActionParameterTile
{
    public class Description : GameAction.ParameterDescription
    {
        public int RangeFromInstigator = int.MaxValue;
        public TileFilterFlags Filter;
        public bool IncludeSelf = false;
    }

    [NetSerializable]
    public class Data : GameAction.ParameterData
    {
        public int2 Tile;

        public Data() { }

        public Data(byte parameterIndex, int2 tile)
            : base(parameterIndex)
        {
            Tile = tile;
        }
    }
}
