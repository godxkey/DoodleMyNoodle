// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class DynamicNetSerializationRegistry
{
    public static readonly ulong crc = 15493578145706223616;

    public static readonly Type[] types = new Type[]
    {
        typeof(ApprovedSimInput)
        ,
        typeof(Fix64)
        ,
        typeof(FixMatrix)
        ,
        typeof(FixMatrix2x2)
        ,
        typeof(FixMatrix2x3)
        ,
        typeof(FixMatrix3x2)
        ,
        typeof(FixMatrix3x3)
        ,
        typeof(FixQuaternion)
        ,
        typeof(FixVector2)
        ,
        typeof(FixVector3)
        ,
        typeof(FixVector4)
        ,
        typeof(InputSubmissionId)
        ,
        typeof(NetMessageChatMessage)
        ,
        typeof(NetMessageChatMessageSubmission)
        ,
        typeof(NetMessageClientHello)
        ,
        typeof(NetMessageExample)
        ,
        typeof(NetMessageInputSubmission)
        ,
        typeof(NetMessagePlayerIdAssignment)
        ,
        typeof(NetMessagePlayerJoined)
        ,
        typeof(NetMessagePlayerLeft)
        ,
        typeof(NetMessagePlayerRepertoireSync)
        ,
        typeof(NetMessageSimTick)
        ,
        typeof(PlayerId)
        ,
        typeof(PlayerInfo)
        ,
        typeof(SimBlueprintId)
        ,
        typeof(SimCommandInjectBlueprint)
        ,
        typeof(SimCommandLoadScene)
        ,
        typeof(SimCommandLog)
        ,
        typeof(SimCommandMoveBall)
        ,
        typeof(SimEntityId)
        ,
        typeof(SimInputKeycode)
        ,
        typeof(SimPlayerId)
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
        [typeof(ApprovedSimInput)] = (obj) =>
        {
            ApprovedSimInput castedObj = (ApprovedSimInput)obj;
            return StaticNetSerializer_ApprovedSimInput.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(Fix64)] = (obj) =>
        {
            Fix64 castedObj = (Fix64)obj;
            return StaticNetSerializer_Fix64.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(FixMatrix)] = (obj) =>
        {
            FixMatrix castedObj = (FixMatrix)obj;
            return StaticNetSerializer_FixMatrix.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(FixMatrix2x2)] = (obj) =>
        {
            FixMatrix2x2 castedObj = (FixMatrix2x2)obj;
            return StaticNetSerializer_FixMatrix2x2.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(FixMatrix2x3)] = (obj) =>
        {
            FixMatrix2x3 castedObj = (FixMatrix2x3)obj;
            return StaticNetSerializer_FixMatrix2x3.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(FixMatrix3x2)] = (obj) =>
        {
            FixMatrix3x2 castedObj = (FixMatrix3x2)obj;
            return StaticNetSerializer_FixMatrix3x2.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(FixMatrix3x3)] = (obj) =>
        {
            FixMatrix3x3 castedObj = (FixMatrix3x3)obj;
            return StaticNetSerializer_FixMatrix3x3.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(FixQuaternion)] = (obj) =>
        {
            FixQuaternion castedObj = (FixQuaternion)obj;
            return StaticNetSerializer_FixQuaternion.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(FixVector2)] = (obj) =>
        {
            FixVector2 castedObj = (FixVector2)obj;
            return StaticNetSerializer_FixVector2.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(FixVector3)] = (obj) =>
        {
            FixVector3 castedObj = (FixVector3)obj;
            return StaticNetSerializer_FixVector3.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(FixVector4)] = (obj) =>
        {
            FixVector4 castedObj = (FixVector4)obj;
            return StaticNetSerializer_FixVector4.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(InputSubmissionId)] = (obj) =>
        {
            InputSubmissionId castedObj = (InputSubmissionId)obj;
            return StaticNetSerializer_InputSubmissionId.GetNetBitSize(ref castedObj);
        }
        ,
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
        [typeof(NetMessageInputSubmission)] = (obj) =>
        {
            NetMessageInputSubmission castedObj = (NetMessageInputSubmission)obj;
            return StaticNetSerializer_NetMessageInputSubmission.GetNetBitSize(ref castedObj);
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
        [typeof(NetMessageSimTick)] = (obj) =>
        {
            NetMessageSimTick castedObj = (NetMessageSimTick)obj;
            return StaticNetSerializer_NetMessageSimTick.GetNetBitSize(ref castedObj);
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
        [typeof(SimBlueprintId)] = (obj) =>
        {
            SimBlueprintId castedObj = (SimBlueprintId)obj;
            return StaticNetSerializer_SimBlueprintId.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(SimCommandInjectBlueprint)] = (obj) =>
        {
            SimCommandInjectBlueprint castedObj = (SimCommandInjectBlueprint)obj;
            return StaticNetSerializer_SimCommandInjectBlueprint.GetNetBitSize(castedObj);
        }
        ,
        [typeof(SimCommandLoadScene)] = (obj) =>
        {
            SimCommandLoadScene castedObj = (SimCommandLoadScene)obj;
            return StaticNetSerializer_SimCommandLoadScene.GetNetBitSize(castedObj);
        }
        ,
        [typeof(SimCommandLog)] = (obj) =>
        {
            SimCommandLog castedObj = (SimCommandLog)obj;
            return StaticNetSerializer_SimCommandLog.GetNetBitSize(castedObj);
        }
        ,
        [typeof(SimCommandMoveBall)] = (obj) =>
        {
            SimCommandMoveBall castedObj = (SimCommandMoveBall)obj;
            return StaticNetSerializer_SimCommandMoveBall.GetNetBitSize(castedObj);
        }
        ,
        [typeof(SimEntityId)] = (obj) =>
        {
            SimEntityId castedObj = (SimEntityId)obj;
            return StaticNetSerializer_SimEntityId.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(SimInputKeycode)] = (obj) =>
        {
            SimInputKeycode castedObj = (SimInputKeycode)obj;
            return StaticNetSerializer_SimInputKeycode.GetNetBitSize(castedObj);
        }
        ,
        [typeof(SimPlayerId)] = (obj) =>
        {
            SimPlayerId castedObj = (SimPlayerId)obj;
            return StaticNetSerializer_SimPlayerId.GetNetBitSize(ref castedObj);
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
        [typeof(ApprovedSimInput)] = (obj, writer) =>
        {
            ApprovedSimInput castedObj = (ApprovedSimInput)obj;
            StaticNetSerializer_ApprovedSimInput.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(Fix64)] = (obj, writer) =>
        {
            Fix64 castedObj = (Fix64)obj;
            StaticNetSerializer_Fix64.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(FixMatrix)] = (obj, writer) =>
        {
            FixMatrix castedObj = (FixMatrix)obj;
            StaticNetSerializer_FixMatrix.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(FixMatrix2x2)] = (obj, writer) =>
        {
            FixMatrix2x2 castedObj = (FixMatrix2x2)obj;
            StaticNetSerializer_FixMatrix2x2.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(FixMatrix2x3)] = (obj, writer) =>
        {
            FixMatrix2x3 castedObj = (FixMatrix2x3)obj;
            StaticNetSerializer_FixMatrix2x3.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(FixMatrix3x2)] = (obj, writer) =>
        {
            FixMatrix3x2 castedObj = (FixMatrix3x2)obj;
            StaticNetSerializer_FixMatrix3x2.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(FixMatrix3x3)] = (obj, writer) =>
        {
            FixMatrix3x3 castedObj = (FixMatrix3x3)obj;
            StaticNetSerializer_FixMatrix3x3.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(FixQuaternion)] = (obj, writer) =>
        {
            FixQuaternion castedObj = (FixQuaternion)obj;
            StaticNetSerializer_FixQuaternion.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(FixVector2)] = (obj, writer) =>
        {
            FixVector2 castedObj = (FixVector2)obj;
            StaticNetSerializer_FixVector2.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(FixVector3)] = (obj, writer) =>
        {
            FixVector3 castedObj = (FixVector3)obj;
            StaticNetSerializer_FixVector3.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(FixVector4)] = (obj, writer) =>
        {
            FixVector4 castedObj = (FixVector4)obj;
            StaticNetSerializer_FixVector4.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(InputSubmissionId)] = (obj, writer) =>
        {
            InputSubmissionId castedObj = (InputSubmissionId)obj;
            StaticNetSerializer_InputSubmissionId.NetSerialize(ref castedObj, writer);
        }
        ,
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
        [typeof(NetMessageInputSubmission)] = (obj, writer) =>
        {
            NetMessageInputSubmission castedObj = (NetMessageInputSubmission)obj;
            StaticNetSerializer_NetMessageInputSubmission.NetSerialize(ref castedObj, writer);
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
        [typeof(NetMessageSimTick)] = (obj, writer) =>
        {
            NetMessageSimTick castedObj = (NetMessageSimTick)obj;
            StaticNetSerializer_NetMessageSimTick.NetSerialize(ref castedObj, writer);
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
        [typeof(SimBlueprintId)] = (obj, writer) =>
        {
            SimBlueprintId castedObj = (SimBlueprintId)obj;
            StaticNetSerializer_SimBlueprintId.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(SimCommandInjectBlueprint)] = (obj, writer) =>
        {
            SimCommandInjectBlueprint castedObj = (SimCommandInjectBlueprint)obj;
            StaticNetSerializer_SimCommandInjectBlueprint.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(SimCommandLoadScene)] = (obj, writer) =>
        {
            SimCommandLoadScene castedObj = (SimCommandLoadScene)obj;
            StaticNetSerializer_SimCommandLoadScene.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(SimCommandLog)] = (obj, writer) =>
        {
            SimCommandLog castedObj = (SimCommandLog)obj;
            StaticNetSerializer_SimCommandLog.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(SimCommandMoveBall)] = (obj, writer) =>
        {
            SimCommandMoveBall castedObj = (SimCommandMoveBall)obj;
            StaticNetSerializer_SimCommandMoveBall.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(SimEntityId)] = (obj, writer) =>
        {
            SimEntityId castedObj = (SimEntityId)obj;
            StaticNetSerializer_SimEntityId.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(SimInputKeycode)] = (obj, writer) =>
        {
            SimInputKeycode castedObj = (SimInputKeycode)obj;
            StaticNetSerializer_SimInputKeycode.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(SimPlayerId)] = (obj, writer) =>
        {
            SimPlayerId castedObj = (SimPlayerId)obj;
            StaticNetSerializer_SimPlayerId.NetSerialize(ref castedObj, writer);
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
            ApprovedSimInput obj = new ApprovedSimInput();
            StaticNetSerializer_ApprovedSimInput.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [1] = (reader) =>
        {
            Fix64 obj = new Fix64();
            StaticNetSerializer_Fix64.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [2] = (reader) =>
        {
            FixMatrix obj = new FixMatrix();
            StaticNetSerializer_FixMatrix.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [3] = (reader) =>
        {
            FixMatrix2x2 obj = new FixMatrix2x2();
            StaticNetSerializer_FixMatrix2x2.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [4] = (reader) =>
        {
            FixMatrix2x3 obj = new FixMatrix2x3();
            StaticNetSerializer_FixMatrix2x3.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [5] = (reader) =>
        {
            FixMatrix3x2 obj = new FixMatrix3x2();
            StaticNetSerializer_FixMatrix3x2.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [6] = (reader) =>
        {
            FixMatrix3x3 obj = new FixMatrix3x3();
            StaticNetSerializer_FixMatrix3x3.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [7] = (reader) =>
        {
            FixQuaternion obj = new FixQuaternion();
            StaticNetSerializer_FixQuaternion.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [8] = (reader) =>
        {
            FixVector2 obj = new FixVector2();
            StaticNetSerializer_FixVector2.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [9] = (reader) =>
        {
            FixVector3 obj = new FixVector3();
            StaticNetSerializer_FixVector3.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [10] = (reader) =>
        {
            FixVector4 obj = new FixVector4();
            StaticNetSerializer_FixVector4.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [11] = (reader) =>
        {
            InputSubmissionId obj = new InputSubmissionId();
            StaticNetSerializer_InputSubmissionId.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [12] = (reader) =>
        {
            NetMessageChatMessage obj = new NetMessageChatMessage();
            StaticNetSerializer_NetMessageChatMessage.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [13] = (reader) =>
        {
            NetMessageChatMessageSubmission obj = new NetMessageChatMessageSubmission();
            StaticNetSerializer_NetMessageChatMessageSubmission.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [14] = (reader) =>
        {
            NetMessageClientHello obj = new NetMessageClientHello();
            StaticNetSerializer_NetMessageClientHello.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [15] = (reader) =>
        {
            NetMessageExample obj = new NetMessageExample();
            StaticNetSerializer_NetMessageExample.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [16] = (reader) =>
        {
            NetMessageInputSubmission obj = new NetMessageInputSubmission();
            StaticNetSerializer_NetMessageInputSubmission.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [17] = (reader) =>
        {
            NetMessagePlayerIdAssignment obj = new NetMessagePlayerIdAssignment();
            StaticNetSerializer_NetMessagePlayerIdAssignment.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [18] = (reader) =>
        {
            NetMessagePlayerJoined obj = new NetMessagePlayerJoined();
            StaticNetSerializer_NetMessagePlayerJoined.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [19] = (reader) =>
        {
            NetMessagePlayerLeft obj = new NetMessagePlayerLeft();
            StaticNetSerializer_NetMessagePlayerLeft.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [20] = (reader) =>
        {
            NetMessagePlayerRepertoireSync obj = new NetMessagePlayerRepertoireSync();
            StaticNetSerializer_NetMessagePlayerRepertoireSync.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [21] = (reader) =>
        {
            NetMessageSimTick obj = new NetMessageSimTick();
            StaticNetSerializer_NetMessageSimTick.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [22] = (reader) =>
        {
            PlayerId obj = new PlayerId();
            StaticNetSerializer_PlayerId.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [23] = (reader) =>
        {
            PlayerInfo obj = new PlayerInfo();
            StaticNetSerializer_PlayerInfo.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [24] = (reader) =>
        {
            SimBlueprintId obj = new SimBlueprintId();
            StaticNetSerializer_SimBlueprintId.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [25] = (reader) =>
        {
            SimCommandInjectBlueprint obj = new SimCommandInjectBlueprint();
            StaticNetSerializer_SimCommandInjectBlueprint.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [26] = (reader) =>
        {
            SimCommandLoadScene obj = new SimCommandLoadScene();
            StaticNetSerializer_SimCommandLoadScene.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [27] = (reader) =>
        {
            SimCommandLog obj = new SimCommandLog();
            StaticNetSerializer_SimCommandLog.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [28] = (reader) =>
        {
            SimCommandMoveBall obj = new SimCommandMoveBall();
            StaticNetSerializer_SimCommandMoveBall.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [29] = (reader) =>
        {
            SimEntityId obj = new SimEntityId();
            StaticNetSerializer_SimEntityId.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [30] = (reader) =>
        {
            SimInputKeycode obj = new SimInputKeycode();
            StaticNetSerializer_SimInputKeycode.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [31] = (reader) =>
        {
            SimPlayerId obj = new SimPlayerId();
            StaticNetSerializer_SimPlayerId.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [32] = (reader) =>
        {
            TestMessage obj = new TestMessage();
            StaticNetSerializer_TestMessage.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [33] = (reader) =>
        {
            TestMessageAnimal obj = new TestMessageAnimal();
            StaticNetSerializer_TestMessageAnimal.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [34] = (reader) =>
        {
            TestMessageCat obj = new TestMessageCat();
            StaticNetSerializer_TestMessageCat.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [35] = (reader) =>
        {
            TestMessageDog obj = new TestMessageDog();
            StaticNetSerializer_TestMessageDog.NetDeserialize(obj, reader);
            return obj;
        }
    };
}
