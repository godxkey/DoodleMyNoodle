// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;

public static class NetMessageRegistry
{
    public static readonly ulong crc = 2568793850405757041;

    public static readonly Type[] types = new Type[]
    {
        typeof(IgnoreThisClass)
        ,typeof(NetMessageClientHello)
        ,typeof(NetMessageExample)
        ,typeof(NetMessagePlayerIdAssignment)
        ,typeof(NetMessagePlayerJoined)
        ,typeof(NetMessagePlayerLeft)
        ,typeof(NetMessagePlayerRepertoireSync)
        ,typeof(PlayerId)
        ,typeof(PlayerInfo)
    };

    public static readonly Factory<UInt16, INetSerializable> factory = new Factory<UInt16, INetSerializable>(new ValueTuple<UInt16, Func<INetSerializable>>[]
    {
        (0, ()=> new IgnoreThisClass())
        ,(1, ()=> new NetMessageClientHello())
        ,(2, ()=> new NetMessageExample())
        ,(3, ()=> new NetMessagePlayerIdAssignment())
        ,(4, ()=> new NetMessagePlayerJoined())
        ,(5, ()=> new NetMessagePlayerLeft())
        ,(6, ()=> new NetMessagePlayerRepertoireSync())
        ,(7, ()=> new PlayerId())
        ,(8, ()=> new PlayerInfo())
    });
}
