// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class DynamicNetSerializationRegistry
{
    public static readonly ulong crc = 7690745408028585420;

    public static readonly Type[] types = new Type[]
    {
        typeof(CCC.Online.DataTransfer.NetMessageCancel)
        ,
        typeof(CCC.Online.DataTransfer.NetMessagePacket)
        ,
        typeof(CCC.Online.DataTransfer.NetMessagePacketACK)
        ,
        typeof(CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader)
        ,
        typeof(CCC.Online.DataTransfer.NetMessageViaStreamACK)
        ,
        typeof(CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader)
        ,
        typeof(CCC.Online.DataTransfer.NetMessageViaStreamReady)
        ,
        typeof(fix)
        ,
        typeof(fix2)
        ,
        typeof(fix2x2)
        ,
        typeof(fix2x3)
        ,
        typeof(fix3)
        ,
        typeof(fix3x2)
        ,
        typeof(fix3x3)
        ,
        typeof(fix4)
        ,
        typeof(fix4x4)
        ,
        typeof(fixQuaternion)
        ,
        typeof(GameAction.UseData)
        ,
        typeof(GameActionParameterTile.Data)
        ,
        typeof(InputSubmissionId)
        ,
        typeof(NetMessageAcceptSimSync)
        ,
        typeof(NetMessageChatMessage)
        ,
        typeof(NetMessageChatMessageSubmission)
        ,
        typeof(NetMessageClientHello)
        ,
        typeof(NetMessageDestroyValue)
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
        typeof(NetMessageRequestSimSync)
        ,
        typeof(NetMessageRequestValueSync)
        ,
        typeof(NetMessageSerializedSimulation)
        ,
        typeof(NetMessageSimPlayerIdAssignement)
        ,
        typeof(NetMessageSimSyncFromFile)
        ,
        typeof(NetMessageSyncValue)
        ,
        typeof(NetMessageValueSyncComplete)
        ,
        typeof(PersistentId)
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
        typeof(SimInputKeycode)
        ,
        typeof(SimInputPlayerCreate)
        ,
        typeof(SimInputPlayerCreateOld)
        ,
        typeof(SimInputPlayerRemove)
        ,
        typeof(SimInputPlayerUpdate)
        ,
        typeof(SimInputSubmission)
        ,
        typeof(SimObjectId)
        ,
        typeof(SimPlayerId)
        ,
        typeof(SimPlayerInfo)
        ,
        typeof(SimPlayerInput)
        ,
        typeof(SimPlayerInputUseItem)
        ,
        typeof(SimTileId_OLD)
        ,
        typeof(SimulationControl.NetMessageSimTick)
        ,
        typeof(SimulationControl.SimTickData)
        ,
        typeof(SyncedValueCurrentLevel)
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
        [typeof(CCC.Online.DataTransfer.NetMessageCancel)] = (obj) =>
        {
            CCC.Online.DataTransfer.NetMessageCancel castedObj = (CCC.Online.DataTransfer.NetMessageCancel)obj;
            return StaticNetSerializer_CCC_Online_DataTransfer_NetMessageCancel.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessagePacket)] = (obj) =>
        {
            CCC.Online.DataTransfer.NetMessagePacket castedObj = (CCC.Online.DataTransfer.NetMessagePacket)obj;
            return StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacket.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessagePacketACK)] = (obj) =>
        {
            CCC.Online.DataTransfer.NetMessagePacketACK castedObj = (CCC.Online.DataTransfer.NetMessagePacketACK)obj;
            return StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacketACK.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader)] = (obj) =>
        {
            CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader castedObj = (CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader)obj;
            return StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaManualPacketsHeader.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaStreamACK)] = (obj) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamACK castedObj = (CCC.Online.DataTransfer.NetMessageViaStreamACK)obj;
            return StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamACK.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader)] = (obj) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader castedObj = (CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader)obj;
            return StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamChannelHeader.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaStreamReady)] = (obj) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamReady castedObj = (CCC.Online.DataTransfer.NetMessageViaStreamReady)obj;
            return StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamReady.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(fix)] = (obj) =>
        {
            fix castedObj = (fix)obj;
            return StaticNetSerializer_fix.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(fix2)] = (obj) =>
        {
            fix2 castedObj = (fix2)obj;
            return StaticNetSerializer_fix2.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(fix2x2)] = (obj) =>
        {
            fix2x2 castedObj = (fix2x2)obj;
            return StaticNetSerializer_fix2x2.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(fix2x3)] = (obj) =>
        {
            fix2x3 castedObj = (fix2x3)obj;
            return StaticNetSerializer_fix2x3.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(fix3)] = (obj) =>
        {
            fix3 castedObj = (fix3)obj;
            return StaticNetSerializer_fix3.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(fix3x2)] = (obj) =>
        {
            fix3x2 castedObj = (fix3x2)obj;
            return StaticNetSerializer_fix3x2.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(fix3x3)] = (obj) =>
        {
            fix3x3 castedObj = (fix3x3)obj;
            return StaticNetSerializer_fix3x3.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(fix4)] = (obj) =>
        {
            fix4 castedObj = (fix4)obj;
            return StaticNetSerializer_fix4.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(fix4x4)] = (obj) =>
        {
            fix4x4 castedObj = (fix4x4)obj;
            return StaticNetSerializer_fix4x4.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(fixQuaternion)] = (obj) =>
        {
            fixQuaternion castedObj = (fixQuaternion)obj;
            return StaticNetSerializer_fixQuaternion.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(GameAction.UseData)] = (obj) =>
        {
            GameAction.UseData castedObj = (GameAction.UseData)obj;
            return StaticNetSerializer_GameAction_UseData.GetNetBitSize(castedObj);
        }
        ,
        [typeof(GameActionParameterTile.Data)] = (obj) =>
        {
            GameActionParameterTile.Data castedObj = (GameActionParameterTile.Data)obj;
            return StaticNetSerializer_GameActionParameterTile_Data.GetNetBitSize(castedObj);
        }
        ,
        [typeof(InputSubmissionId)] = (obj) =>
        {
            InputSubmissionId castedObj = (InputSubmissionId)obj;
            return StaticNetSerializer_InputSubmissionId.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageAcceptSimSync)] = (obj) =>
        {
            NetMessageAcceptSimSync castedObj = (NetMessageAcceptSimSync)obj;
            return StaticNetSerializer_NetMessageAcceptSimSync.GetNetBitSize(ref castedObj);
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
        [typeof(NetMessageDestroyValue)] = (obj) =>
        {
            NetMessageDestroyValue castedObj = (NetMessageDestroyValue)obj;
            return StaticNetSerializer_NetMessageDestroyValue.GetNetBitSize(ref castedObj);
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
        [typeof(NetMessageRequestSimSync)] = (obj) =>
        {
            NetMessageRequestSimSync castedObj = (NetMessageRequestSimSync)obj;
            return StaticNetSerializer_NetMessageRequestSimSync.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageRequestValueSync)] = (obj) =>
        {
            NetMessageRequestValueSync castedObj = (NetMessageRequestValueSync)obj;
            return StaticNetSerializer_NetMessageRequestValueSync.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageSerializedSimulation)] = (obj) =>
        {
            NetMessageSerializedSimulation castedObj = (NetMessageSerializedSimulation)obj;
            return StaticNetSerializer_NetMessageSerializedSimulation.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageSimPlayerIdAssignement)] = (obj) =>
        {
            NetMessageSimPlayerIdAssignement castedObj = (NetMessageSimPlayerIdAssignement)obj;
            return StaticNetSerializer_NetMessageSimPlayerIdAssignement.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageSimSyncFromFile)] = (obj) =>
        {
            NetMessageSimSyncFromFile castedObj = (NetMessageSimSyncFromFile)obj;
            return StaticNetSerializer_NetMessageSimSyncFromFile.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageSyncValue)] = (obj) =>
        {
            NetMessageSyncValue castedObj = (NetMessageSyncValue)obj;
            return StaticNetSerializer_NetMessageSyncValue.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageValueSyncComplete)] = (obj) =>
        {
            NetMessageValueSyncComplete castedObj = (NetMessageValueSyncComplete)obj;
            return StaticNetSerializer_NetMessageValueSyncComplete.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(PersistentId)] = (obj) =>
        {
            PersistentId castedObj = (PersistentId)obj;
            return StaticNetSerializer_PersistentId.GetNetBitSize(ref castedObj);
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
        [typeof(SimInputKeycode)] = (obj) =>
        {
            SimInputKeycode castedObj = (SimInputKeycode)obj;
            return StaticNetSerializer_SimInputKeycode.GetNetBitSize(castedObj);
        }
        ,
        [typeof(SimInputPlayerCreate)] = (obj) =>
        {
            SimInputPlayerCreate castedObj = (SimInputPlayerCreate)obj;
            return StaticNetSerializer_SimInputPlayerCreate.GetNetBitSize(castedObj);
        }
        ,
        [typeof(SimInputPlayerCreateOld)] = (obj) =>
        {
            SimInputPlayerCreateOld castedObj = (SimInputPlayerCreateOld)obj;
            return StaticNetSerializer_SimInputPlayerCreateOld.GetNetBitSize(castedObj);
        }
        ,
        [typeof(SimInputPlayerRemove)] = (obj) =>
        {
            SimInputPlayerRemove castedObj = (SimInputPlayerRemove)obj;
            return StaticNetSerializer_SimInputPlayerRemove.GetNetBitSize(castedObj);
        }
        ,
        [typeof(SimInputPlayerUpdate)] = (obj) =>
        {
            SimInputPlayerUpdate castedObj = (SimInputPlayerUpdate)obj;
            return StaticNetSerializer_SimInputPlayerUpdate.GetNetBitSize(castedObj);
        }
        ,
        [typeof(SimInputSubmission)] = (obj) =>
        {
            SimInputSubmission castedObj = (SimInputSubmission)obj;
            return StaticNetSerializer_SimInputSubmission.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(SimObjectId)] = (obj) =>
        {
            SimObjectId castedObj = (SimObjectId)obj;
            return StaticNetSerializer_SimObjectId.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(SimPlayerId)] = (obj) =>
        {
            SimPlayerId castedObj = (SimPlayerId)obj;
            return StaticNetSerializer_SimPlayerId.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(SimPlayerInfo)] = (obj) =>
        {
            SimPlayerInfo castedObj = (SimPlayerInfo)obj;
            return StaticNetSerializer_SimPlayerInfo.GetNetBitSize(castedObj);
        }
        ,
        [typeof(SimPlayerInput)] = (obj) =>
        {
            SimPlayerInput castedObj = (SimPlayerInput)obj;
            return StaticNetSerializer_SimPlayerInput.GetNetBitSize(castedObj);
        }
        ,
        [typeof(SimPlayerInputUseItem)] = (obj) =>
        {
            SimPlayerInputUseItem castedObj = (SimPlayerInputUseItem)obj;
            return StaticNetSerializer_SimPlayerInputUseItem.GetNetBitSize(castedObj);
        }
        ,
        [typeof(SimTileId_OLD)] = (obj) =>
        {
            SimTileId_OLD castedObj = (SimTileId_OLD)obj;
            return StaticNetSerializer_SimTileId_OLD.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(SimulationControl.NetMessageSimTick)] = (obj) =>
        {
            SimulationControl.NetMessageSimTick castedObj = (SimulationControl.NetMessageSimTick)obj;
            return StaticNetSerializer_SimulationControl_NetMessageSimTick.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(SimulationControl.SimTickData)] = (obj) =>
        {
            SimulationControl.SimTickData castedObj = (SimulationControl.SimTickData)obj;
            return StaticNetSerializer_SimulationControl_SimTickData.GetNetBitSize(ref castedObj);
        }
        ,
        [typeof(SyncedValueCurrentLevel)] = (obj) =>
        {
            SyncedValueCurrentLevel castedObj = (SyncedValueCurrentLevel)obj;
            return StaticNetSerializer_SyncedValueCurrentLevel.GetNetBitSize(ref castedObj);
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
        [typeof(CCC.Online.DataTransfer.NetMessageCancel)] = (obj, writer) =>
        {
            CCC.Online.DataTransfer.NetMessageCancel castedObj = (CCC.Online.DataTransfer.NetMessageCancel)obj;
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageCancel.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessagePacket)] = (obj, writer) =>
        {
            CCC.Online.DataTransfer.NetMessagePacket castedObj = (CCC.Online.DataTransfer.NetMessagePacket)obj;
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacket.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessagePacketACK)] = (obj, writer) =>
        {
            CCC.Online.DataTransfer.NetMessagePacketACK castedObj = (CCC.Online.DataTransfer.NetMessagePacketACK)obj;
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacketACK.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader)] = (obj, writer) =>
        {
            CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader castedObj = (CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader)obj;
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaManualPacketsHeader.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaStreamACK)] = (obj, writer) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamACK castedObj = (CCC.Online.DataTransfer.NetMessageViaStreamACK)obj;
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamACK.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader)] = (obj, writer) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader castedObj = (CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader)obj;
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamChannelHeader.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaStreamReady)] = (obj, writer) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamReady castedObj = (CCC.Online.DataTransfer.NetMessageViaStreamReady)obj;
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamReady.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(fix)] = (obj, writer) =>
        {
            fix castedObj = (fix)obj;
            StaticNetSerializer_fix.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(fix2)] = (obj, writer) =>
        {
            fix2 castedObj = (fix2)obj;
            StaticNetSerializer_fix2.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(fix2x2)] = (obj, writer) =>
        {
            fix2x2 castedObj = (fix2x2)obj;
            StaticNetSerializer_fix2x2.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(fix2x3)] = (obj, writer) =>
        {
            fix2x3 castedObj = (fix2x3)obj;
            StaticNetSerializer_fix2x3.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(fix3)] = (obj, writer) =>
        {
            fix3 castedObj = (fix3)obj;
            StaticNetSerializer_fix3.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(fix3x2)] = (obj, writer) =>
        {
            fix3x2 castedObj = (fix3x2)obj;
            StaticNetSerializer_fix3x2.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(fix3x3)] = (obj, writer) =>
        {
            fix3x3 castedObj = (fix3x3)obj;
            StaticNetSerializer_fix3x3.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(fix4)] = (obj, writer) =>
        {
            fix4 castedObj = (fix4)obj;
            StaticNetSerializer_fix4.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(fix4x4)] = (obj, writer) =>
        {
            fix4x4 castedObj = (fix4x4)obj;
            StaticNetSerializer_fix4x4.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(fixQuaternion)] = (obj, writer) =>
        {
            fixQuaternion castedObj = (fixQuaternion)obj;
            StaticNetSerializer_fixQuaternion.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(GameAction.UseData)] = (obj, writer) =>
        {
            GameAction.UseData castedObj = (GameAction.UseData)obj;
            StaticNetSerializer_GameAction_UseData.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(GameActionParameterTile.Data)] = (obj, writer) =>
        {
            GameActionParameterTile.Data castedObj = (GameActionParameterTile.Data)obj;
            StaticNetSerializer_GameActionParameterTile_Data.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(InputSubmissionId)] = (obj, writer) =>
        {
            InputSubmissionId castedObj = (InputSubmissionId)obj;
            StaticNetSerializer_InputSubmissionId.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageAcceptSimSync)] = (obj, writer) =>
        {
            NetMessageAcceptSimSync castedObj = (NetMessageAcceptSimSync)obj;
            StaticNetSerializer_NetMessageAcceptSimSync.NetSerialize(ref castedObj, writer);
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
        [typeof(NetMessageDestroyValue)] = (obj, writer) =>
        {
            NetMessageDestroyValue castedObj = (NetMessageDestroyValue)obj;
            StaticNetSerializer_NetMessageDestroyValue.NetSerialize(ref castedObj, writer);
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
        [typeof(NetMessageRequestSimSync)] = (obj, writer) =>
        {
            NetMessageRequestSimSync castedObj = (NetMessageRequestSimSync)obj;
            StaticNetSerializer_NetMessageRequestSimSync.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageRequestValueSync)] = (obj, writer) =>
        {
            NetMessageRequestValueSync castedObj = (NetMessageRequestValueSync)obj;
            StaticNetSerializer_NetMessageRequestValueSync.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageSerializedSimulation)] = (obj, writer) =>
        {
            NetMessageSerializedSimulation castedObj = (NetMessageSerializedSimulation)obj;
            StaticNetSerializer_NetMessageSerializedSimulation.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageSimPlayerIdAssignement)] = (obj, writer) =>
        {
            NetMessageSimPlayerIdAssignement castedObj = (NetMessageSimPlayerIdAssignement)obj;
            StaticNetSerializer_NetMessageSimPlayerIdAssignement.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageSimSyncFromFile)] = (obj, writer) =>
        {
            NetMessageSimSyncFromFile castedObj = (NetMessageSimSyncFromFile)obj;
            StaticNetSerializer_NetMessageSimSyncFromFile.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageSyncValue)] = (obj, writer) =>
        {
            NetMessageSyncValue castedObj = (NetMessageSyncValue)obj;
            StaticNetSerializer_NetMessageSyncValue.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageValueSyncComplete)] = (obj, writer) =>
        {
            NetMessageValueSyncComplete castedObj = (NetMessageValueSyncComplete)obj;
            StaticNetSerializer_NetMessageValueSyncComplete.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(PersistentId)] = (obj, writer) =>
        {
            PersistentId castedObj = (PersistentId)obj;
            StaticNetSerializer_PersistentId.NetSerialize(ref castedObj, writer);
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
        [typeof(SimInputKeycode)] = (obj, writer) =>
        {
            SimInputKeycode castedObj = (SimInputKeycode)obj;
            StaticNetSerializer_SimInputKeycode.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(SimInputPlayerCreate)] = (obj, writer) =>
        {
            SimInputPlayerCreate castedObj = (SimInputPlayerCreate)obj;
            StaticNetSerializer_SimInputPlayerCreate.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(SimInputPlayerCreateOld)] = (obj, writer) =>
        {
            SimInputPlayerCreateOld castedObj = (SimInputPlayerCreateOld)obj;
            StaticNetSerializer_SimInputPlayerCreateOld.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(SimInputPlayerRemove)] = (obj, writer) =>
        {
            SimInputPlayerRemove castedObj = (SimInputPlayerRemove)obj;
            StaticNetSerializer_SimInputPlayerRemove.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(SimInputPlayerUpdate)] = (obj, writer) =>
        {
            SimInputPlayerUpdate castedObj = (SimInputPlayerUpdate)obj;
            StaticNetSerializer_SimInputPlayerUpdate.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(SimInputSubmission)] = (obj, writer) =>
        {
            SimInputSubmission castedObj = (SimInputSubmission)obj;
            StaticNetSerializer_SimInputSubmission.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(SimObjectId)] = (obj, writer) =>
        {
            SimObjectId castedObj = (SimObjectId)obj;
            StaticNetSerializer_SimObjectId.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(SimPlayerId)] = (obj, writer) =>
        {
            SimPlayerId castedObj = (SimPlayerId)obj;
            StaticNetSerializer_SimPlayerId.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(SimPlayerInfo)] = (obj, writer) =>
        {
            SimPlayerInfo castedObj = (SimPlayerInfo)obj;
            StaticNetSerializer_SimPlayerInfo.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(SimPlayerInput)] = (obj, writer) =>
        {
            SimPlayerInput castedObj = (SimPlayerInput)obj;
            StaticNetSerializer_SimPlayerInput.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(SimPlayerInputUseItem)] = (obj, writer) =>
        {
            SimPlayerInputUseItem castedObj = (SimPlayerInputUseItem)obj;
            StaticNetSerializer_SimPlayerInputUseItem.NetSerialize(castedObj, writer);
        }
        ,
        [typeof(SimTileId_OLD)] = (obj, writer) =>
        {
            SimTileId_OLD castedObj = (SimTileId_OLD)obj;
            StaticNetSerializer_SimTileId_OLD.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(SimulationControl.NetMessageSimTick)] = (obj, writer) =>
        {
            SimulationControl.NetMessageSimTick castedObj = (SimulationControl.NetMessageSimTick)obj;
            StaticNetSerializer_SimulationControl_NetMessageSimTick.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(SimulationControl.SimTickData)] = (obj, writer) =>
        {
            SimulationControl.SimTickData castedObj = (SimulationControl.SimTickData)obj;
            StaticNetSerializer_SimulationControl_SimTickData.NetSerialize(ref castedObj, writer);
        }
        ,
        [typeof(SyncedValueCurrentLevel)] = (obj, writer) =>
        {
            SyncedValueCurrentLevel castedObj = (SyncedValueCurrentLevel)obj;
            StaticNetSerializer_SyncedValueCurrentLevel.NetSerialize(ref castedObj, writer);
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
            CCC.Online.DataTransfer.NetMessageCancel obj = new CCC.Online.DataTransfer.NetMessageCancel();
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageCancel.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [1] = (reader) =>
        {
            CCC.Online.DataTransfer.NetMessagePacket obj = new CCC.Online.DataTransfer.NetMessagePacket();
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacket.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [2] = (reader) =>
        {
            CCC.Online.DataTransfer.NetMessagePacketACK obj = new CCC.Online.DataTransfer.NetMessagePacketACK();
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacketACK.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [3] = (reader) =>
        {
            CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader obj = new CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader();
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaManualPacketsHeader.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [4] = (reader) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamACK obj = new CCC.Online.DataTransfer.NetMessageViaStreamACK();
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamACK.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [5] = (reader) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader obj = new CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader();
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamChannelHeader.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [6] = (reader) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamReady obj = new CCC.Online.DataTransfer.NetMessageViaStreamReady();
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamReady.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [7] = (reader) =>
        {
            fix obj = new fix();
            StaticNetSerializer_fix.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [8] = (reader) =>
        {
            fix2 obj = new fix2();
            StaticNetSerializer_fix2.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [9] = (reader) =>
        {
            fix2x2 obj = new fix2x2();
            StaticNetSerializer_fix2x2.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [10] = (reader) =>
        {
            fix2x3 obj = new fix2x3();
            StaticNetSerializer_fix2x3.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [11] = (reader) =>
        {
            fix3 obj = new fix3();
            StaticNetSerializer_fix3.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [12] = (reader) =>
        {
            fix3x2 obj = new fix3x2();
            StaticNetSerializer_fix3x2.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [13] = (reader) =>
        {
            fix3x3 obj = new fix3x3();
            StaticNetSerializer_fix3x3.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [14] = (reader) =>
        {
            fix4 obj = new fix4();
            StaticNetSerializer_fix4.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [15] = (reader) =>
        {
            fix4x4 obj = new fix4x4();
            StaticNetSerializer_fix4x4.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [16] = (reader) =>
        {
            fixQuaternion obj = new fixQuaternion();
            StaticNetSerializer_fixQuaternion.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [17] = (reader) =>
        {
            GameAction.UseData obj = new GameAction.UseData();
            StaticNetSerializer_GameAction_UseData.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [18] = (reader) =>
        {
            GameActionParameterTile.Data obj = new GameActionParameterTile.Data();
            StaticNetSerializer_GameActionParameterTile_Data.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [19] = (reader) =>
        {
            InputSubmissionId obj = new InputSubmissionId();
            StaticNetSerializer_InputSubmissionId.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [20] = (reader) =>
        {
            NetMessageAcceptSimSync obj = new NetMessageAcceptSimSync();
            StaticNetSerializer_NetMessageAcceptSimSync.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [21] = (reader) =>
        {
            NetMessageChatMessage obj = new NetMessageChatMessage();
            StaticNetSerializer_NetMessageChatMessage.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [22] = (reader) =>
        {
            NetMessageChatMessageSubmission obj = new NetMessageChatMessageSubmission();
            StaticNetSerializer_NetMessageChatMessageSubmission.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [23] = (reader) =>
        {
            NetMessageClientHello obj = new NetMessageClientHello();
            StaticNetSerializer_NetMessageClientHello.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [24] = (reader) =>
        {
            NetMessageDestroyValue obj = new NetMessageDestroyValue();
            StaticNetSerializer_NetMessageDestroyValue.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [25] = (reader) =>
        {
            NetMessageExample obj = new NetMessageExample();
            StaticNetSerializer_NetMessageExample.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [26] = (reader) =>
        {
            NetMessageInputSubmission obj = new NetMessageInputSubmission();
            StaticNetSerializer_NetMessageInputSubmission.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [27] = (reader) =>
        {
            NetMessagePlayerIdAssignment obj = new NetMessagePlayerIdAssignment();
            StaticNetSerializer_NetMessagePlayerIdAssignment.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [28] = (reader) =>
        {
            NetMessagePlayerJoined obj = new NetMessagePlayerJoined();
            StaticNetSerializer_NetMessagePlayerJoined.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [29] = (reader) =>
        {
            NetMessagePlayerLeft obj = new NetMessagePlayerLeft();
            StaticNetSerializer_NetMessagePlayerLeft.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [30] = (reader) =>
        {
            NetMessagePlayerRepertoireSync obj = new NetMessagePlayerRepertoireSync();
            StaticNetSerializer_NetMessagePlayerRepertoireSync.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [31] = (reader) =>
        {
            NetMessageRequestSimSync obj = new NetMessageRequestSimSync();
            StaticNetSerializer_NetMessageRequestSimSync.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [32] = (reader) =>
        {
            NetMessageRequestValueSync obj = new NetMessageRequestValueSync();
            StaticNetSerializer_NetMessageRequestValueSync.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [33] = (reader) =>
        {
            NetMessageSerializedSimulation obj = new NetMessageSerializedSimulation();
            StaticNetSerializer_NetMessageSerializedSimulation.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [34] = (reader) =>
        {
            NetMessageSimPlayerIdAssignement obj = new NetMessageSimPlayerIdAssignement();
            StaticNetSerializer_NetMessageSimPlayerIdAssignement.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [35] = (reader) =>
        {
            NetMessageSimSyncFromFile obj = new NetMessageSimSyncFromFile();
            StaticNetSerializer_NetMessageSimSyncFromFile.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [36] = (reader) =>
        {
            NetMessageSyncValue obj = new NetMessageSyncValue();
            StaticNetSerializer_NetMessageSyncValue.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [37] = (reader) =>
        {
            NetMessageValueSyncComplete obj = new NetMessageValueSyncComplete();
            StaticNetSerializer_NetMessageValueSyncComplete.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [38] = (reader) =>
        {
            PersistentId obj = new PersistentId();
            StaticNetSerializer_PersistentId.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [39] = (reader) =>
        {
            PlayerId obj = new PlayerId();
            StaticNetSerializer_PlayerId.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [40] = (reader) =>
        {
            PlayerInfo obj = new PlayerInfo();
            StaticNetSerializer_PlayerInfo.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [41] = (reader) =>
        {
            SimBlueprintId obj = new SimBlueprintId();
            StaticNetSerializer_SimBlueprintId.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [42] = (reader) =>
        {
            SimCommandInjectBlueprint obj = new SimCommandInjectBlueprint();
            StaticNetSerializer_SimCommandInjectBlueprint.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [43] = (reader) =>
        {
            SimCommandLoadScene obj = new SimCommandLoadScene();
            StaticNetSerializer_SimCommandLoadScene.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [44] = (reader) =>
        {
            SimCommandLog obj = new SimCommandLog();
            StaticNetSerializer_SimCommandLog.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [45] = (reader) =>
        {
            SimInputKeycode obj = new SimInputKeycode();
            StaticNetSerializer_SimInputKeycode.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [46] = (reader) =>
        {
            SimInputPlayerCreate obj = new SimInputPlayerCreate();
            StaticNetSerializer_SimInputPlayerCreate.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [47] = (reader) =>
        {
            SimInputPlayerCreateOld obj = new SimInputPlayerCreateOld();
            StaticNetSerializer_SimInputPlayerCreateOld.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [48] = (reader) =>
        {
            SimInputPlayerRemove obj = new SimInputPlayerRemove();
            StaticNetSerializer_SimInputPlayerRemove.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [49] = (reader) =>
        {
            SimInputPlayerUpdate obj = new SimInputPlayerUpdate();
            StaticNetSerializer_SimInputPlayerUpdate.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [50] = (reader) =>
        {
            SimInputSubmission obj = new SimInputSubmission();
            StaticNetSerializer_SimInputSubmission.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [51] = (reader) =>
        {
            SimObjectId obj = new SimObjectId();
            StaticNetSerializer_SimObjectId.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [52] = (reader) =>
        {
            SimPlayerId obj = new SimPlayerId();
            StaticNetSerializer_SimPlayerId.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [53] = (reader) =>
        {
            SimPlayerInfo obj = new SimPlayerInfo();
            StaticNetSerializer_SimPlayerInfo.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [54] = (reader) =>
        {
            SimPlayerInput obj = new SimPlayerInput();
            StaticNetSerializer_SimPlayerInput.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [55] = (reader) =>
        {
            SimPlayerInputUseItem obj = new SimPlayerInputUseItem();
            StaticNetSerializer_SimPlayerInputUseItem.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [56] = (reader) =>
        {
            SimTileId_OLD obj = new SimTileId_OLD();
            StaticNetSerializer_SimTileId_OLD.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [57] = (reader) =>
        {
            SimulationControl.NetMessageSimTick obj = new SimulationControl.NetMessageSimTick();
            StaticNetSerializer_SimulationControl_NetMessageSimTick.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [58] = (reader) =>
        {
            SimulationControl.SimTickData obj = new SimulationControl.SimTickData();
            StaticNetSerializer_SimulationControl_SimTickData.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [59] = (reader) =>
        {
            SyncedValueCurrentLevel obj = new SyncedValueCurrentLevel();
            StaticNetSerializer_SyncedValueCurrentLevel.NetDeserialize(ref obj, reader);
            return obj;
        }
        ,
        [60] = (reader) =>
        {
            TestMessage obj = new TestMessage();
            StaticNetSerializer_TestMessage.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [61] = (reader) =>
        {
            TestMessageAnimal obj = new TestMessageAnimal();
            StaticNetSerializer_TestMessageAnimal.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [62] = (reader) =>
        {
            TestMessageCat obj = new TestMessageCat();
            StaticNetSerializer_TestMessageCat.NetDeserialize(obj, reader);
            return obj;
        }
        ,
        [63] = (reader) =>
        {
            TestMessageDog obj = new TestMessageDog();
            StaticNetSerializer_TestMessageDog.NetDeserialize(obj, reader);
            return obj;
        }
    };
}
