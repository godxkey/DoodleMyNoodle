// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class DynamicNetSerializationRegistry
{
    public static readonly ulong crc = 13454997013868263077;

    public static readonly Type[] types = new Type[]
    {
        typeof(NetMessageChatMessage)
        ,
        typeof(NetMessageChatMessageSubmission)
        ,
        typeof(NetMessageClientHello)
        ,
        typeof(NetMessageExample)
        ,
        typeof(NetMessagePlayerIdAssignment)
        ,
        typeof(NetMessagePlayerJoined)
        ,
        typeof(NetMessagePlayerLeft)
        ,
        typeof(NetMessagePlayerRepertoireSync)
        ,
        typeof(PlayerId)
        ,
        typeof(PlayerInfo)
        ,
        typeof(TestMessage)
        ,
        typeof(TestMessageAnimal)
        ,
        typeof(TestMessageCat)
        ,
        typeof(TestMessageDog)
    };

    public static readonly Dictionary<Type, Func<object, int>> map_GetBitSize = new Dictionary<Type, Func<object, int>>()
    {
        [typeof(NetMessageChatMessage)] = (obj) =>
        {
            NetMessageChatMessage castedObj = (NetMessageChatMessage)obj;
            return StaticNetSerializer_NetMessageChatMessage.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageChatMessageSubmission)] = (obj) =>
        {
            NetMessageChatMessageSubmission castedObj = (NetMessageChatMessageSubmission)obj;
            return StaticNetSerializer_NetMessageChatMessageSubmission.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageClientHello)] = (obj) =>
        {
            NetMessageClientHello castedObj = (NetMessageClientHello)obj;
            return StaticNetSerializer_NetMessageClientHello.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageExample)] = (obj) =>
        {
            NetMessageExample castedObj = (NetMessageExample)obj;
            return StaticNetSerializer_NetMessageExample.GetNetBitSize(castedObj);
        }
        ,
        [typeof(NetMessagePlayerIdAssignment)] = (obj) =>
        {
            NetMessagePlayerIdAssignment castedObj = (NetMessagePlayerIdAssignment)obj;
            return StaticNetSerializer_NetMessagePlayerIdAssignment.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessagePlayerJoined)] = (obj) =>
        {
            NetMessagePlayerJoined castedObj = (NetMessagePlayerJoined)obj;
            return StaticNetSerializer_NetMessagePlayerJoined.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessagePlayerLeft)] = (obj) =>
        {
            NetMessagePlayerLeft castedObj = (NetMessagePlayerLeft)obj;
            return StaticNetSerializer_NetMessagePlayerLeft.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessagePlayerRepertoireSync)] = (obj) =>
        {
            NetMessagePlayerRepertoireSync castedObj = (NetMessagePlayerRepertoireSync)obj;
            return StaticNetSerializer_NetMessagePlayerRepertoireSync.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(PlayerId)] = (obj) =>
        {
            PlayerId castedObj = (PlayerId)obj;
            return StaticNetSerializer_PlayerId.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(PlayerInfo)] = (obj) =>
        {
            PlayerInfo castedObj = (PlayerInfo)obj;
            return StaticNetSerializer_PlayerInfo.GetNetBitSize(castedObj);
        }
        ,
        [typeof(TestMessage)] = (obj) =>
        {
            TestMessage castedObj = (TestMessage)obj;
            return StaticNetSerializer_TestMessage.GetNetBitSize(castedObj);
        }
        ,
        [typeof(TestMessageAnimal)] = (obj) =>
        {
            TestMessageAnimal castedObj = (TestMessageAnimal)obj;
            return StaticNetSerializer_TestMessageAnimal.GetNetBitSize(castedObj);
        }
        ,
        [typeof(TestMessageCat)] = (obj) =>
        {
            TestMessageCat castedObj = (TestMessageCat)obj;
            return StaticNetSerializer_TestMessageCat.GetNetBitSize(castedObj);
        }
        ,
        [typeof(TestMessageDog)] = (obj) =>
        {
            TestMessageDog castedObj = (TestMessageDog)obj;
            return StaticNetSerializer_TestMessageDog.GetNetBitSize(castedObj);
        }
    };

    public static readonly Dictionary<Type, Action<object, BitStreamWriter>> map_Serialize = new Dictionary<Type, Action<object, BitStreamWriter>>()
    {
        [typeof(NetMessageChatMessage)] = (obj, writer) =>
        {
            NetMessageChatMessage castedObj = (NetMessageChatMessage)obj;
            StaticNetSerializer_NetMessageChatMessage.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageChatMessageSubmission)] = (obj, writer) =>
        {
            NetMessageChatMessageSubmission castedObj = (NetMessageChatMessageSubmission)obj;
            StaticNetSerializer_NetMessageChatMessageSubmission.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageClientHello)] = (obj, writer) =>
        {
            NetMessageClientHello castedObj = (NetMessageClientHello)obj;
            StaticNetSerializer_NetMessageClientHello.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageExample)] = (obj, writer) =>
        {
            NetMessageExample castedObj = (NetMessageExample)obj;
            StaticNetSerializer_NetMessageExample.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(NetMessagePlayerIdAssignment)] = (obj, writer) =>
        {
            NetMessagePlayerIdAssignment castedObj = (NetMessagePlayerIdAssignment)obj;
            StaticNetSerializer_NetMessagePlayerIdAssignment.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessagePlayerJoined)] = (obj, writer) =>
        {
            NetMessagePlayerJoined castedObj = (NetMessagePlayerJoined)obj;
            StaticNetSerializer_NetMessagePlayerJoined.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessagePlayerLeft)] = (obj, writer) =>
        {
            NetMessagePlayerLeft castedObj = (NetMessagePlayerLeft)obj;
            StaticNetSerializer_NetMessagePlayerLeft.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessagePlayerRepertoireSync)] = (obj, writer) =>
        {
            NetMessagePlayerRepertoireSync castedObj = (NetMessagePlayerRepertoireSync)obj;
            StaticNetSerializer_NetMessagePlayerRepertoireSync.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(PlayerId)] = (obj, writer) =>
        {
            PlayerId castedObj = (PlayerId)obj;
            StaticNetSerializer_PlayerId.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(PlayerInfo)] = (obj, writer) =>
        {
            PlayerInfo castedObj = (PlayerInfo)obj;
            StaticNetSerializer_PlayerInfo.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(TestMessage)] = (obj, writer) =>
        {
            TestMessage castedObj = (TestMessage)obj;
            StaticNetSerializer_TestMessage.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(TestMessageAnimal)] = (obj, writer) =>
        {
            TestMessageAnimal castedObj = (TestMessageAnimal)obj;
            StaticNetSerializer_TestMessageAnimal.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(TestMessageCat)] = (obj, writer) =>
        {
            TestMessageCat castedObj = (TestMessageCat)obj;
            StaticNetSerializer_TestMessageCat.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(TestMessageDog)] = (obj, writer) =>
        {
            TestMessageDog castedObj = (TestMessageDog)obj;
            StaticNetSerializer_TestMessageDog.NetSerialize(castedObj, writer);
        }
    };

    public static readonly Dictionary<UInt16, Func<BitStreamReader, object>> map_Deserialize = new Dictionary<UInt16, Func<BitStreamReader, object>>()
    {
        [0] = (reader) =>
        {
            NetMessageChatMessage obj = new NetMessageChatMessage();
            StaticNetSerializer_NetMessageChatMessage.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [1] = (reader) =>
        {
            NetMessageChatMessageSubmission obj = new NetMessageChatMessageSubmission();
            StaticNetSerializer_NetMessageChatMessageSubmission.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [2] = (reader) =>
        {
            NetMessageClientHello obj = new NetMessageClientHello();
            StaticNetSerializer_NetMessageClientHello.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [3] = (reader) =>
        {
            NetMessageExample obj = new NetMessageExample();
            StaticNetSerializer_NetMessageExample.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [4] = (reader) =>
        {
            NetMessagePlayerIdAssignment obj = new NetMessagePlayerIdAssignment();
            StaticNetSerializer_NetMessagePlayerIdAssignment.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [5] = (reader) =>
        {
            NetMessagePlayerJoined obj = new NetMessagePlayerJoined();
            StaticNetSerializer_NetMessagePlayerJoined.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [6] = (reader) =>
        {
            NetMessagePlayerLeft obj = new NetMessagePlayerLeft();
            StaticNetSerializer_NetMessagePlayerLeft.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [7] = (reader) =>
        {
            NetMessagePlayerRepertoireSync obj = new NetMessagePlayerRepertoireSync();
            StaticNetSerializer_NetMessagePlayerRepertoireSync.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [8] = (reader) =>
        {
            PlayerId obj = new PlayerId();
            StaticNetSerializer_PlayerId.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [9] = (reader) =>
        {
            PlayerInfo obj = new PlayerInfo();
            StaticNetSerializer_PlayerInfo.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [10] = (reader) =>
        {
            TestMessage obj = new TestMessage();
            StaticNetSerializer_TestMessage.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [11] = (reader) =>
        {
            TestMessageAnimal obj = new TestMessageAnimal();
            StaticNetSerializer_TestMessageAnimal.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [12] = (reader) =>
        {
            TestMessageCat obj = new TestMessageCat();
            StaticNetSerializer_TestMessageCat.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [13] = (reader) =>
        {
            TestMessageDog obj = new TestMessageDog();
            StaticNetSerializer_TestMessageDog.NetDeserialize(obj, reader);
            return obj;
        }
    };
}
