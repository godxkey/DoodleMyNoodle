
[System.Serializable]
[NetSerializable]
public class SimPlayerInfo : ISimPlayerInfo
{
    public string Name;
    public SimPlayerId SimPlayerId;

    public SimPlayerInfo() { }
    public SimPlayerInfo(SimPlayerInfo other)
    {
        Name = other.Name;
        SimPlayerId = other.SimPlayerId;
    }

    // read-only interface
    string ISimPlayerInfo.Name => Name;
    SimPlayerId ISimPlayerInfo.SimPlayerId => SimPlayerId;
}

public interface ISimPlayerInfo // Read-only version
{
    string Name { get; }
    SimPlayerId SimPlayerId { get; }
}