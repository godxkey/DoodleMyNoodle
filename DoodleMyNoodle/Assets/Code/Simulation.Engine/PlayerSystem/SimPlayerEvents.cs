
public struct SimPlayerCreatedEventData
{
    public ISimPlayerInfo PlayerInfo;
}

public struct SimPlayerUpdatedEvent
{
    public ISimPlayerInfo PlayerInfo;
}

public struct SimPlayerDestroyedEventData
{
    public SimPlayerId SimPlayerId;
}