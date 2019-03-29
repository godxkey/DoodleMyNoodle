// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;

public static class NetMessageRegistry
{
    public static readonly ulong crc = 14250364852035444353;

    public static readonly Type[] types = new Type[]
    {
        typeof(IgnoreThisClass)
        ,typeof(NetMessageExample)
        ,typeof(PlayerId)
        ,typeof(PlayerIdAssignment)
        ,typeof(PlayerInfo)
        ,typeof(PlayerRepertoire)
    };

    public static readonly Factory<UInt16, INetSerializable> factory = new Factory<UInt16, INetSerializable>(new ValueTuple<UInt16, Func<INetSerializable>>[]
    {
        (0, ()=> new IgnoreThisClass())
        ,(1, ()=> new NetMessageExample())
        ,(2, ()=> new PlayerId())
        ,(3, ()=> new PlayerIdAssignment())
        ,(4, ()=> new PlayerInfo())
        ,(5, ()=> new PlayerRepertoire())
    });
}
