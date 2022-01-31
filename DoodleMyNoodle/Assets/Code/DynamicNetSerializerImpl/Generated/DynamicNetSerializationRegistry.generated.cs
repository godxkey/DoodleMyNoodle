// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class DynamicNetSerializationRegistry
{
    public static readonly ulong crc = 10747419460602381265;

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
        typeof(CCC.Online.DataTransfer.NetMessageViaStreamUpdate)
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
        typeof(GameAction.UseParameters)
        ,
        typeof(GameActionParameterBool.Data)
        ,
        typeof(GameActionParameterEntity.Data)
        ,
        typeof(GameActionParameterPosition.Data)
        ,
        typeof(GameActionParameterSuccessRate.Data)
        ,
        typeof(GameActionParameterTile.Data)
        ,
        typeof(GameActionParameterVector.Data)
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
        typeof(NetMessagePlayerAssets)
        ,
        typeof(NetMessagePlayerAssets.Data)
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
        typeof(Sim.Operations.SerializedWorld)
        ,
        typeof(Sim.Operations.SerializedWorld.BlobAsset)
        ,
        typeof(SimCommandLoadScene)
        ,
        typeof(SimInputCheatAddAllItems)
        ,
        typeof(SimInputCheatDamageSelf)
        ,
        typeof(SimInputCheatImpulseSelf)
        ,
        typeof(SimInputCheatRemoveAllCooldowns)
        ,
        typeof(SimInputCheatTeleport)
        ,
        typeof(SimInputCheatToggleInvincible)
        ,
        typeof(SimInputPlayerCreate)
        ,
        typeof(SimInputSetPlayerActive)
        ,
        typeof(SimInputSubmission)
        ,
        typeof(SimPlayerInputClickSignalEmitter)
        ,
        typeof(SimPlayerInputDropItem)
        ,
        typeof(SimPlayerInputEquipItem)
        ,
        typeof(SimPlayerInputJump)
        ,
        typeof(SimPlayerInputMove)
        ,
        typeof(SimPlayerInputNextTurn)
        ,
        typeof(SimPlayerInputSelectStartingInventory)
        ,
        typeof(SimPlayerInputSetPawnDoodle)
        ,
        typeof(SimPlayerInputSetPawnName)
        ,
        typeof(SimPlayerInputUseItem)
        ,
        typeof(SimPlayerInputUseObjectGameAction)
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
        ,
        typeof(Unity.Mathematics.int2)
        ,
        typeof(UnityEngine.Vector2)
        ,
        typeof(UnityEngine.Vector3)
        ,
        typeof(UnityEngine.Vector4)
    };

    public static readonly Dictionary<Type, Func<object, int>> map_GetBitSize = new Dictionary<Type, Func<object, int>>()
    {
        [typeof(CCC.Online.DataTransfer.NetMessageCancel)] = (obj) =>
        {
            CCC.Online.DataTransfer.NetMessageCancel castedObj = (CCC.Online.DataTransfer.NetMessageCancel)obj;
            return StaticNetSerializer_CCC_Online_DataTransfer_NetMessageCancel.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessagePacket)] = (obj) =>
        {
            CCC.Online.DataTransfer.NetMessagePacket castedObj = (CCC.Online.DataTransfer.NetMessagePacket)obj;
            return StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacket.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessagePacketACK)] = (obj) =>
        {
            CCC.Online.DataTransfer.NetMessagePacketACK castedObj = (CCC.Online.DataTransfer.NetMessagePacketACK)obj;
            return StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacketACK.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader)] = (obj) =>
        {
            CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader castedObj = (CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader)obj;
            return StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaManualPacketsHeader.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaStreamACK)] = (obj) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamACK castedObj = (CCC.Online.DataTransfer.NetMessageViaStreamACK)obj;
            return StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamACK.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader)] = (obj) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader castedObj = (CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader)obj;
            return StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamChannelHeader.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaStreamReady)] = (obj) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamReady castedObj = (CCC.Online.DataTransfer.NetMessageViaStreamReady)obj;
            return StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamReady.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaStreamUpdate)] = (obj) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamUpdate castedObj = (CCC.Online.DataTransfer.NetMessageViaStreamUpdate)obj;
            return StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamUpdate.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(fix)] = (obj) =>
        {
            fix castedObj = (fix)obj;
            return StaticNetSerializer_fix.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(fix2)] = (obj) =>
        {
            fix2 castedObj = (fix2)obj;
            return StaticNetSerializer_fix2.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(fix2x2)] = (obj) =>
        {
            fix2x2 castedObj = (fix2x2)obj;
            return StaticNetSerializer_fix2x2.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(fix2x3)] = (obj) =>
        {
            fix2x3 castedObj = (fix2x3)obj;
            return StaticNetSerializer_fix2x3.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(fix3)] = (obj) =>
        {
            fix3 castedObj = (fix3)obj;
            return StaticNetSerializer_fix3.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(fix3x2)] = (obj) =>
        {
            fix3x2 castedObj = (fix3x2)obj;
            return StaticNetSerializer_fix3x2.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(fix3x3)] = (obj) =>
        {
            fix3x3 castedObj = (fix3x3)obj;
            return StaticNetSerializer_fix3x3.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(fix4)] = (obj) =>
        {
            fix4 castedObj = (fix4)obj;
            return StaticNetSerializer_fix4.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(fix4x4)] = (obj) =>
        {
            fix4x4 castedObj = (fix4x4)obj;
            return StaticNetSerializer_fix4x4.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(fixQuaternion)] = (obj) =>
        {
            fixQuaternion castedObj = (fixQuaternion)obj;
            return StaticNetSerializer_fixQuaternion.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(GameAction.UseParameters)] = (obj) =>
        {
            GameAction.UseParameters castedObj = (GameAction.UseParameters)obj;
            return StaticNetSerializer_GameAction_UseParameters.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(GameActionParameterBool.Data)] = (obj) =>
        {
            GameActionParameterBool.Data castedObj = (GameActionParameterBool.Data)obj;
            return StaticNetSerializer_GameActionParameterBool_Data.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(GameActionParameterEntity.Data)] = (obj) =>
        {
            GameActionParameterEntity.Data castedObj = (GameActionParameterEntity.Data)obj;
            return StaticNetSerializer_GameActionParameterEntity_Data.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(GameActionParameterPosition.Data)] = (obj) =>
        {
            GameActionParameterPosition.Data castedObj = (GameActionParameterPosition.Data)obj;
            return StaticNetSerializer_GameActionParameterPosition_Data.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(GameActionParameterSuccessRate.Data)] = (obj) =>
        {
            GameActionParameterSuccessRate.Data castedObj = (GameActionParameterSuccessRate.Data)obj;
            return StaticNetSerializer_GameActionParameterSuccessRate_Data.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(GameActionParameterTile.Data)] = (obj) =>
        {
            GameActionParameterTile.Data castedObj = (GameActionParameterTile.Data)obj;
            return StaticNetSerializer_GameActionParameterTile_Data.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(GameActionParameterVector.Data)] = (obj) =>
        {
            GameActionParameterVector.Data castedObj = (GameActionParameterVector.Data)obj;
            return StaticNetSerializer_GameActionParameterVector_Data.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(InputSubmissionId)] = (obj) =>
        {
            InputSubmissionId castedObj = (InputSubmissionId)obj;
            return StaticNetSerializer_InputSubmissionId.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageAcceptSimSync)] = (obj) =>
        {
            NetMessageAcceptSimSync castedObj = (NetMessageAcceptSimSync)obj;
            return StaticNetSerializer_NetMessageAcceptSimSync.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageChatMessage)] = (obj) =>
        {
            NetMessageChatMessage castedObj = (NetMessageChatMessage)obj;
            return StaticNetSerializer_NetMessageChatMessage.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageChatMessageSubmission)] = (obj) =>
        {
            NetMessageChatMessageSubmission castedObj = (NetMessageChatMessageSubmission)obj;
            return StaticNetSerializer_NetMessageChatMessageSubmission.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageClientHello)] = (obj) =>
        {
            NetMessageClientHello castedObj = (NetMessageClientHello)obj;
            return StaticNetSerializer_NetMessageClientHello.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageDestroyValue)] = (obj) =>
        {
            NetMessageDestroyValue castedObj = (NetMessageDestroyValue)obj;
            return StaticNetSerializer_NetMessageDestroyValue.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageExample)] = (obj) =>
        {
            NetMessageExample castedObj = (NetMessageExample)obj;
            return StaticNetSerializer_NetMessageExample.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(NetMessageInputSubmission)] = (obj) =>
        {
            NetMessageInputSubmission castedObj = (NetMessageInputSubmission)obj;
            return StaticNetSerializer_NetMessageInputSubmission.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessagePlayerAssets)] = (obj) =>
        {
            NetMessagePlayerAssets castedObj = (NetMessagePlayerAssets)obj;
            return StaticNetSerializer_NetMessagePlayerAssets.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessagePlayerAssets.Data)] = (obj) =>
        {
            NetMessagePlayerAssets.Data castedObj = (NetMessagePlayerAssets.Data)obj;
            return StaticNetSerializer_NetMessagePlayerAssets_Data.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessagePlayerIdAssignment)] = (obj) =>
        {
            NetMessagePlayerIdAssignment castedObj = (NetMessagePlayerIdAssignment)obj;
            return StaticNetSerializer_NetMessagePlayerIdAssignment.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessagePlayerJoined)] = (obj) =>
        {
            NetMessagePlayerJoined castedObj = (NetMessagePlayerJoined)obj;
            return StaticNetSerializer_NetMessagePlayerJoined.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessagePlayerLeft)] = (obj) =>
        {
            NetMessagePlayerLeft castedObj = (NetMessagePlayerLeft)obj;
            return StaticNetSerializer_NetMessagePlayerLeft.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessagePlayerRepertoireSync)] = (obj) =>
        {
            NetMessagePlayerRepertoireSync castedObj = (NetMessagePlayerRepertoireSync)obj;
            return StaticNetSerializer_NetMessagePlayerRepertoireSync.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageRequestSimSync)] = (obj) =>
        {
            NetMessageRequestSimSync castedObj = (NetMessageRequestSimSync)obj;
            return StaticNetSerializer_NetMessageRequestSimSync.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageRequestValueSync)] = (obj) =>
        {
            NetMessageRequestValueSync castedObj = (NetMessageRequestValueSync)obj;
            return StaticNetSerializer_NetMessageRequestValueSync.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageSerializedSimulation)] = (obj) =>
        {
            NetMessageSerializedSimulation castedObj = (NetMessageSerializedSimulation)obj;
            return StaticNetSerializer_NetMessageSerializedSimulation.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageSimPlayerIdAssignement)] = (obj) =>
        {
            NetMessageSimPlayerIdAssignement castedObj = (NetMessageSimPlayerIdAssignement)obj;
            return StaticNetSerializer_NetMessageSimPlayerIdAssignement.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageSimSyncFromFile)] = (obj) =>
        {
            NetMessageSimSyncFromFile castedObj = (NetMessageSimSyncFromFile)obj;
            return StaticNetSerializer_NetMessageSimSyncFromFile.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageSyncValue)] = (obj) =>
        {
            NetMessageSyncValue castedObj = (NetMessageSyncValue)obj;
            return StaticNetSerializer_NetMessageSyncValue.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(NetMessageValueSyncComplete)] = (obj) =>
        {
            NetMessageValueSyncComplete castedObj = (NetMessageValueSyncComplete)obj;
            return StaticNetSerializer_NetMessageValueSyncComplete.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(PersistentId)] = (obj) =>
        {
            PersistentId castedObj = (PersistentId)obj;
            return StaticNetSerializer_PersistentId.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(PlayerId)] = (obj) =>
        {
            PlayerId castedObj = (PlayerId)obj;
            return StaticNetSerializer_PlayerId.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(PlayerInfo)] = (obj) =>
        {
            PlayerInfo castedObj = (PlayerInfo)obj;
            return StaticNetSerializer_PlayerInfo.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(Sim.Operations.SerializedWorld)] = (obj) =>
        {
            Sim.Operations.SerializedWorld castedObj = (Sim.Operations.SerializedWorld)obj;
            return StaticNetSerializer_Sim_Operations_SerializedWorld.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(Sim.Operations.SerializedWorld.BlobAsset)] = (obj) =>
        {
            Sim.Operations.SerializedWorld.BlobAsset castedObj = (Sim.Operations.SerializedWorld.BlobAsset)obj;
            return StaticNetSerializer_Sim_Operations_SerializedWorld_BlobAsset.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimCommandLoadScene)] = (obj) =>
        {
            SimCommandLoadScene castedObj = (SimCommandLoadScene)obj;
            return StaticNetSerializer_SimCommandLoadScene.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimInputCheatAddAllItems)] = (obj) =>
        {
            SimInputCheatAddAllItems castedObj = (SimInputCheatAddAllItems)obj;
            return StaticNetSerializer_SimInputCheatAddAllItems.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimInputCheatDamageSelf)] = (obj) =>
        {
            SimInputCheatDamageSelf castedObj = (SimInputCheatDamageSelf)obj;
            return StaticNetSerializer_SimInputCheatDamageSelf.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimInputCheatImpulseSelf)] = (obj) =>
        {
            SimInputCheatImpulseSelf castedObj = (SimInputCheatImpulseSelf)obj;
            return StaticNetSerializer_SimInputCheatImpulseSelf.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimInputCheatRemoveAllCooldowns)] = (obj) =>
        {
            SimInputCheatRemoveAllCooldowns castedObj = (SimInputCheatRemoveAllCooldowns)obj;
            return StaticNetSerializer_SimInputCheatRemoveAllCooldowns.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimInputCheatTeleport)] = (obj) =>
        {
            SimInputCheatTeleport castedObj = (SimInputCheatTeleport)obj;
            return StaticNetSerializer_SimInputCheatTeleport.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimInputCheatToggleInvincible)] = (obj) =>
        {
            SimInputCheatToggleInvincible castedObj = (SimInputCheatToggleInvincible)obj;
            return StaticNetSerializer_SimInputCheatToggleInvincible.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimInputPlayerCreate)] = (obj) =>
        {
            SimInputPlayerCreate castedObj = (SimInputPlayerCreate)obj;
            return StaticNetSerializer_SimInputPlayerCreate.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimInputSetPlayerActive)] = (obj) =>
        {
            SimInputSetPlayerActive castedObj = (SimInputSetPlayerActive)obj;
            return StaticNetSerializer_SimInputSetPlayerActive.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimInputSubmission)] = (obj) =>
        {
            SimInputSubmission castedObj = (SimInputSubmission)obj;
            return StaticNetSerializer_SimInputSubmission.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(SimPlayerInputClickSignalEmitter)] = (obj) =>
        {
            SimPlayerInputClickSignalEmitter castedObj = (SimPlayerInputClickSignalEmitter)obj;
            return StaticNetSerializer_SimPlayerInputClickSignalEmitter.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimPlayerInputDropItem)] = (obj) =>
        {
            SimPlayerInputDropItem castedObj = (SimPlayerInputDropItem)obj;
            return StaticNetSerializer_SimPlayerInputDropItem.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimPlayerInputEquipItem)] = (obj) =>
        {
            SimPlayerInputEquipItem castedObj = (SimPlayerInputEquipItem)obj;
            return StaticNetSerializer_SimPlayerInputEquipItem.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimPlayerInputJump)] = (obj) =>
        {
            SimPlayerInputJump castedObj = (SimPlayerInputJump)obj;
            return StaticNetSerializer_SimPlayerInputJump.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimPlayerInputMove)] = (obj) =>
        {
            SimPlayerInputMove castedObj = (SimPlayerInputMove)obj;
            return StaticNetSerializer_SimPlayerInputMove.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimPlayerInputNextTurn)] = (obj) =>
        {
            SimPlayerInputNextTurn castedObj = (SimPlayerInputNextTurn)obj;
            return StaticNetSerializer_SimPlayerInputNextTurn.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimPlayerInputSelectStartingInventory)] = (obj) =>
        {
            SimPlayerInputSelectStartingInventory castedObj = (SimPlayerInputSelectStartingInventory)obj;
            return StaticNetSerializer_SimPlayerInputSelectStartingInventory.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimPlayerInputSetPawnDoodle)] = (obj) =>
        {
            SimPlayerInputSetPawnDoodle castedObj = (SimPlayerInputSetPawnDoodle)obj;
            return StaticNetSerializer_SimPlayerInputSetPawnDoodle.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimPlayerInputSetPawnName)] = (obj) =>
        {
            SimPlayerInputSetPawnName castedObj = (SimPlayerInputSetPawnName)obj;
            return StaticNetSerializer_SimPlayerInputSetPawnName.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimPlayerInputUseItem)] = (obj) =>
        {
            SimPlayerInputUseItem castedObj = (SimPlayerInputUseItem)obj;
            return StaticNetSerializer_SimPlayerInputUseItem.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimPlayerInputUseObjectGameAction)] = (obj) =>
        {
            SimPlayerInputUseObjectGameAction castedObj = (SimPlayerInputUseObjectGameAction)obj;
            return StaticNetSerializer_SimPlayerInputUseObjectGameAction.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(SimulationControl.NetMessageSimTick)] = (obj) =>
        {
            SimulationControl.NetMessageSimTick castedObj = (SimulationControl.NetMessageSimTick)obj;
            return StaticNetSerializer_SimulationControl_NetMessageSimTick.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(SimulationControl.SimTickData)] = (obj) =>
        {
            SimulationControl.SimTickData castedObj = (SimulationControl.SimTickData)obj;
            return StaticNetSerializer_SimulationControl_SimTickData.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(SyncedValueCurrentLevel)] = (obj) =>
        {
            SyncedValueCurrentLevel castedObj = (SyncedValueCurrentLevel)obj;
            return StaticNetSerializer_SyncedValueCurrentLevel.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(TestMessage)] = (obj) =>
        {
            TestMessage castedObj = (TestMessage)obj;
            return StaticNetSerializer_TestMessage.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(TestMessageAnimal)] = (obj) =>
        {
            TestMessageAnimal castedObj = (TestMessageAnimal)obj;
            return StaticNetSerializer_TestMessageAnimal.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(TestMessageCat)] = (obj) =>
        {
            TestMessageCat castedObj = (TestMessageCat)obj;
            return StaticNetSerializer_TestMessageCat.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(TestMessageDog)] = (obj) =>
        {
            TestMessageDog castedObj = (TestMessageDog)obj;
            return StaticNetSerializer_TestMessageDog.GetSerializedBitSize(castedObj);
        }
        ,
        [typeof(Unity.Mathematics.int2)] = (obj) =>
        {
            Unity.Mathematics.int2 castedObj = (Unity.Mathematics.int2)obj;
            return StaticNetSerializer_Unity_Mathematics_int2.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(UnityEngine.Vector2)] = (obj) =>
        {
            UnityEngine.Vector2 castedObj = (UnityEngine.Vector2)obj;
            return StaticNetSerializer_UnityEngine_Vector2.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(UnityEngine.Vector3)] = (obj) =>
        {
            UnityEngine.Vector3 castedObj = (UnityEngine.Vector3)obj;
            return StaticNetSerializer_UnityEngine_Vector3.GetSerializedBitSize(ref castedObj);
        }
        ,
        [typeof(UnityEngine.Vector4)] = (obj) =>
        {
            UnityEngine.Vector4 castedObj = (UnityEngine.Vector4)obj;
            return StaticNetSerializer_UnityEngine_Vector4.GetSerializedBitSize(ref castedObj);
        }
    };

    public static readonly Dictionary<Type, Action<object, BitStreamWriter>> map_Serialize = new Dictionary<Type, Action<object, BitStreamWriter>>()
    {
        [typeof(CCC.Online.DataTransfer.NetMessageCancel)] = (obj, writer) =>
        {
            CCC.Online.DataTransfer.NetMessageCancel castedObj = (CCC.Online.DataTransfer.NetMessageCancel)obj;
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageCancel.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessagePacket)] = (obj, writer) =>
        {
            CCC.Online.DataTransfer.NetMessagePacket castedObj = (CCC.Online.DataTransfer.NetMessagePacket)obj;
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacket.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessagePacketACK)] = (obj, writer) =>
        {
            CCC.Online.DataTransfer.NetMessagePacketACK castedObj = (CCC.Online.DataTransfer.NetMessagePacketACK)obj;
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacketACK.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader)] = (obj, writer) =>
        {
            CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader castedObj = (CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader)obj;
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaManualPacketsHeader.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaStreamACK)] = (obj, writer) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamACK castedObj = (CCC.Online.DataTransfer.NetMessageViaStreamACK)obj;
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamACK.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader)] = (obj, writer) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader castedObj = (CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader)obj;
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamChannelHeader.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaStreamReady)] = (obj, writer) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamReady castedObj = (CCC.Online.DataTransfer.NetMessageViaStreamReady)obj;
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamReady.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(CCC.Online.DataTransfer.NetMessageViaStreamUpdate)] = (obj, writer) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamUpdate castedObj = (CCC.Online.DataTransfer.NetMessageViaStreamUpdate)obj;
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamUpdate.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(fix)] = (obj, writer) =>
        {
            fix castedObj = (fix)obj;
            StaticNetSerializer_fix.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(fix2)] = (obj, writer) =>
        {
            fix2 castedObj = (fix2)obj;
            StaticNetSerializer_fix2.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(fix2x2)] = (obj, writer) =>
        {
            fix2x2 castedObj = (fix2x2)obj;
            StaticNetSerializer_fix2x2.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(fix2x3)] = (obj, writer) =>
        {
            fix2x3 castedObj = (fix2x3)obj;
            StaticNetSerializer_fix2x3.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(fix3)] = (obj, writer) =>
        {
            fix3 castedObj = (fix3)obj;
            StaticNetSerializer_fix3.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(fix3x2)] = (obj, writer) =>
        {
            fix3x2 castedObj = (fix3x2)obj;
            StaticNetSerializer_fix3x2.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(fix3x3)] = (obj, writer) =>
        {
            fix3x3 castedObj = (fix3x3)obj;
            StaticNetSerializer_fix3x3.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(fix4)] = (obj, writer) =>
        {
            fix4 castedObj = (fix4)obj;
            StaticNetSerializer_fix4.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(fix4x4)] = (obj, writer) =>
        {
            fix4x4 castedObj = (fix4x4)obj;
            StaticNetSerializer_fix4x4.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(fixQuaternion)] = (obj, writer) =>
        {
            fixQuaternion castedObj = (fixQuaternion)obj;
            StaticNetSerializer_fixQuaternion.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(GameAction.UseParameters)] = (obj, writer) =>
        {
            GameAction.UseParameters castedObj = (GameAction.UseParameters)obj;
            StaticNetSerializer_GameAction_UseParameters.Serialize(castedObj, writer);
        }
        ,
        [typeof(GameActionParameterBool.Data)] = (obj, writer) =>
        {
            GameActionParameterBool.Data castedObj = (GameActionParameterBool.Data)obj;
            StaticNetSerializer_GameActionParameterBool_Data.Serialize(castedObj, writer);
        }
        ,
        [typeof(GameActionParameterEntity.Data)] = (obj, writer) =>
        {
            GameActionParameterEntity.Data castedObj = (GameActionParameterEntity.Data)obj;
            StaticNetSerializer_GameActionParameterEntity_Data.Serialize(castedObj, writer);
        }
        ,
        [typeof(GameActionParameterPosition.Data)] = (obj, writer) =>
        {
            GameActionParameterPosition.Data castedObj = (GameActionParameterPosition.Data)obj;
            StaticNetSerializer_GameActionParameterPosition_Data.Serialize(castedObj, writer);
        }
        ,
        [typeof(GameActionParameterSuccessRate.Data)] = (obj, writer) =>
        {
            GameActionParameterSuccessRate.Data castedObj = (GameActionParameterSuccessRate.Data)obj;
            StaticNetSerializer_GameActionParameterSuccessRate_Data.Serialize(castedObj, writer);
        }
        ,
        [typeof(GameActionParameterTile.Data)] = (obj, writer) =>
        {
            GameActionParameterTile.Data castedObj = (GameActionParameterTile.Data)obj;
            StaticNetSerializer_GameActionParameterTile_Data.Serialize(castedObj, writer);
        }
        ,
        [typeof(GameActionParameterVector.Data)] = (obj, writer) =>
        {
            GameActionParameterVector.Data castedObj = (GameActionParameterVector.Data)obj;
            StaticNetSerializer_GameActionParameterVector_Data.Serialize(castedObj, writer);
        }
        ,
        [typeof(InputSubmissionId)] = (obj, writer) =>
        {
            InputSubmissionId castedObj = (InputSubmissionId)obj;
            StaticNetSerializer_InputSubmissionId.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageAcceptSimSync)] = (obj, writer) =>
        {
            NetMessageAcceptSimSync castedObj = (NetMessageAcceptSimSync)obj;
            StaticNetSerializer_NetMessageAcceptSimSync.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageChatMessage)] = (obj, writer) =>
        {
            NetMessageChatMessage castedObj = (NetMessageChatMessage)obj;
            StaticNetSerializer_NetMessageChatMessage.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageChatMessageSubmission)] = (obj, writer) =>
        {
            NetMessageChatMessageSubmission castedObj = (NetMessageChatMessageSubmission)obj;
            StaticNetSerializer_NetMessageChatMessageSubmission.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageClientHello)] = (obj, writer) =>
        {
            NetMessageClientHello castedObj = (NetMessageClientHello)obj;
            StaticNetSerializer_NetMessageClientHello.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageDestroyValue)] = (obj, writer) =>
        {
            NetMessageDestroyValue castedObj = (NetMessageDestroyValue)obj;
            StaticNetSerializer_NetMessageDestroyValue.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageExample)] = (obj, writer) =>
        {
            NetMessageExample castedObj = (NetMessageExample)obj;
            StaticNetSerializer_NetMessageExample.Serialize(castedObj, writer);
        }
        ,
        [typeof(NetMessageInputSubmission)] = (obj, writer) =>
        {
            NetMessageInputSubmission castedObj = (NetMessageInputSubmission)obj;
            StaticNetSerializer_NetMessageInputSubmission.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessagePlayerAssets)] = (obj, writer) =>
        {
            NetMessagePlayerAssets castedObj = (NetMessagePlayerAssets)obj;
            StaticNetSerializer_NetMessagePlayerAssets.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessagePlayerAssets.Data)] = (obj, writer) =>
        {
            NetMessagePlayerAssets.Data castedObj = (NetMessagePlayerAssets.Data)obj;
            StaticNetSerializer_NetMessagePlayerAssets_Data.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessagePlayerIdAssignment)] = (obj, writer) =>
        {
            NetMessagePlayerIdAssignment castedObj = (NetMessagePlayerIdAssignment)obj;
            StaticNetSerializer_NetMessagePlayerIdAssignment.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessagePlayerJoined)] = (obj, writer) =>
        {
            NetMessagePlayerJoined castedObj = (NetMessagePlayerJoined)obj;
            StaticNetSerializer_NetMessagePlayerJoined.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessagePlayerLeft)] = (obj, writer) =>
        {
            NetMessagePlayerLeft castedObj = (NetMessagePlayerLeft)obj;
            StaticNetSerializer_NetMessagePlayerLeft.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessagePlayerRepertoireSync)] = (obj, writer) =>
        {
            NetMessagePlayerRepertoireSync castedObj = (NetMessagePlayerRepertoireSync)obj;
            StaticNetSerializer_NetMessagePlayerRepertoireSync.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageRequestSimSync)] = (obj, writer) =>
        {
            NetMessageRequestSimSync castedObj = (NetMessageRequestSimSync)obj;
            StaticNetSerializer_NetMessageRequestSimSync.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageRequestValueSync)] = (obj, writer) =>
        {
            NetMessageRequestValueSync castedObj = (NetMessageRequestValueSync)obj;
            StaticNetSerializer_NetMessageRequestValueSync.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageSerializedSimulation)] = (obj, writer) =>
        {
            NetMessageSerializedSimulation castedObj = (NetMessageSerializedSimulation)obj;
            StaticNetSerializer_NetMessageSerializedSimulation.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageSimPlayerIdAssignement)] = (obj, writer) =>
        {
            NetMessageSimPlayerIdAssignement castedObj = (NetMessageSimPlayerIdAssignement)obj;
            StaticNetSerializer_NetMessageSimPlayerIdAssignement.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageSimSyncFromFile)] = (obj, writer) =>
        {
            NetMessageSimSyncFromFile castedObj = (NetMessageSimSyncFromFile)obj;
            StaticNetSerializer_NetMessageSimSyncFromFile.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageSyncValue)] = (obj, writer) =>
        {
            NetMessageSyncValue castedObj = (NetMessageSyncValue)obj;
            StaticNetSerializer_NetMessageSyncValue.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(NetMessageValueSyncComplete)] = (obj, writer) =>
        {
            NetMessageValueSyncComplete castedObj = (NetMessageValueSyncComplete)obj;
            StaticNetSerializer_NetMessageValueSyncComplete.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(PersistentId)] = (obj, writer) =>
        {
            PersistentId castedObj = (PersistentId)obj;
            StaticNetSerializer_PersistentId.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(PlayerId)] = (obj, writer) =>
        {
            PlayerId castedObj = (PlayerId)obj;
            StaticNetSerializer_PlayerId.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(PlayerInfo)] = (obj, writer) =>
        {
            PlayerInfo castedObj = (PlayerInfo)obj;
            StaticNetSerializer_PlayerInfo.Serialize(castedObj, writer);
        }
        ,
        [typeof(Sim.Operations.SerializedWorld)] = (obj, writer) =>
        {
            Sim.Operations.SerializedWorld castedObj = (Sim.Operations.SerializedWorld)obj;
            StaticNetSerializer_Sim_Operations_SerializedWorld.Serialize(castedObj, writer);
        }
        ,
        [typeof(Sim.Operations.SerializedWorld.BlobAsset)] = (obj, writer) =>
        {
            Sim.Operations.SerializedWorld.BlobAsset castedObj = (Sim.Operations.SerializedWorld.BlobAsset)obj;
            StaticNetSerializer_Sim_Operations_SerializedWorld_BlobAsset.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimCommandLoadScene)] = (obj, writer) =>
        {
            SimCommandLoadScene castedObj = (SimCommandLoadScene)obj;
            StaticNetSerializer_SimCommandLoadScene.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimInputCheatAddAllItems)] = (obj, writer) =>
        {
            SimInputCheatAddAllItems castedObj = (SimInputCheatAddAllItems)obj;
            StaticNetSerializer_SimInputCheatAddAllItems.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimInputCheatDamageSelf)] = (obj, writer) =>
        {
            SimInputCheatDamageSelf castedObj = (SimInputCheatDamageSelf)obj;
            StaticNetSerializer_SimInputCheatDamageSelf.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimInputCheatImpulseSelf)] = (obj, writer) =>
        {
            SimInputCheatImpulseSelf castedObj = (SimInputCheatImpulseSelf)obj;
            StaticNetSerializer_SimInputCheatImpulseSelf.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimInputCheatRemoveAllCooldowns)] = (obj, writer) =>
        {
            SimInputCheatRemoveAllCooldowns castedObj = (SimInputCheatRemoveAllCooldowns)obj;
            StaticNetSerializer_SimInputCheatRemoveAllCooldowns.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimInputCheatTeleport)] = (obj, writer) =>
        {
            SimInputCheatTeleport castedObj = (SimInputCheatTeleport)obj;
            StaticNetSerializer_SimInputCheatTeleport.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimInputCheatToggleInvincible)] = (obj, writer) =>
        {
            SimInputCheatToggleInvincible castedObj = (SimInputCheatToggleInvincible)obj;
            StaticNetSerializer_SimInputCheatToggleInvincible.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimInputPlayerCreate)] = (obj, writer) =>
        {
            SimInputPlayerCreate castedObj = (SimInputPlayerCreate)obj;
            StaticNetSerializer_SimInputPlayerCreate.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimInputSetPlayerActive)] = (obj, writer) =>
        {
            SimInputSetPlayerActive castedObj = (SimInputSetPlayerActive)obj;
            StaticNetSerializer_SimInputSetPlayerActive.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimInputSubmission)] = (obj, writer) =>
        {
            SimInputSubmission castedObj = (SimInputSubmission)obj;
            StaticNetSerializer_SimInputSubmission.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(SimPlayerInputClickSignalEmitter)] = (obj, writer) =>
        {
            SimPlayerInputClickSignalEmitter castedObj = (SimPlayerInputClickSignalEmitter)obj;
            StaticNetSerializer_SimPlayerInputClickSignalEmitter.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimPlayerInputDropItem)] = (obj, writer) =>
        {
            SimPlayerInputDropItem castedObj = (SimPlayerInputDropItem)obj;
            StaticNetSerializer_SimPlayerInputDropItem.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimPlayerInputEquipItem)] = (obj, writer) =>
        {
            SimPlayerInputEquipItem castedObj = (SimPlayerInputEquipItem)obj;
            StaticNetSerializer_SimPlayerInputEquipItem.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimPlayerInputJump)] = (obj, writer) =>
        {
            SimPlayerInputJump castedObj = (SimPlayerInputJump)obj;
            StaticNetSerializer_SimPlayerInputJump.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimPlayerInputMove)] = (obj, writer) =>
        {
            SimPlayerInputMove castedObj = (SimPlayerInputMove)obj;
            StaticNetSerializer_SimPlayerInputMove.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimPlayerInputNextTurn)] = (obj, writer) =>
        {
            SimPlayerInputNextTurn castedObj = (SimPlayerInputNextTurn)obj;
            StaticNetSerializer_SimPlayerInputNextTurn.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimPlayerInputSelectStartingInventory)] = (obj, writer) =>
        {
            SimPlayerInputSelectStartingInventory castedObj = (SimPlayerInputSelectStartingInventory)obj;
            StaticNetSerializer_SimPlayerInputSelectStartingInventory.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimPlayerInputSetPawnDoodle)] = (obj, writer) =>
        {
            SimPlayerInputSetPawnDoodle castedObj = (SimPlayerInputSetPawnDoodle)obj;
            StaticNetSerializer_SimPlayerInputSetPawnDoodle.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimPlayerInputSetPawnName)] = (obj, writer) =>
        {
            SimPlayerInputSetPawnName castedObj = (SimPlayerInputSetPawnName)obj;
            StaticNetSerializer_SimPlayerInputSetPawnName.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimPlayerInputUseItem)] = (obj, writer) =>
        {
            SimPlayerInputUseItem castedObj = (SimPlayerInputUseItem)obj;
            StaticNetSerializer_SimPlayerInputUseItem.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimPlayerInputUseObjectGameAction)] = (obj, writer) =>
        {
            SimPlayerInputUseObjectGameAction castedObj = (SimPlayerInputUseObjectGameAction)obj;
            StaticNetSerializer_SimPlayerInputUseObjectGameAction.Serialize(castedObj, writer);
        }
        ,
        [typeof(SimulationControl.NetMessageSimTick)] = (obj, writer) =>
        {
            SimulationControl.NetMessageSimTick castedObj = (SimulationControl.NetMessageSimTick)obj;
            StaticNetSerializer_SimulationControl_NetMessageSimTick.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(SimulationControl.SimTickData)] = (obj, writer) =>
        {
            SimulationControl.SimTickData castedObj = (SimulationControl.SimTickData)obj;
            StaticNetSerializer_SimulationControl_SimTickData.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(SyncedValueCurrentLevel)] = (obj, writer) =>
        {
            SyncedValueCurrentLevel castedObj = (SyncedValueCurrentLevel)obj;
            StaticNetSerializer_SyncedValueCurrentLevel.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(TestMessage)] = (obj, writer) =>
        {
            TestMessage castedObj = (TestMessage)obj;
            StaticNetSerializer_TestMessage.Serialize(castedObj, writer);
        }
        ,
        [typeof(TestMessageAnimal)] = (obj, writer) =>
        {
            TestMessageAnimal castedObj = (TestMessageAnimal)obj;
            StaticNetSerializer_TestMessageAnimal.Serialize(castedObj, writer);
        }
        ,
        [typeof(TestMessageCat)] = (obj, writer) =>
        {
            TestMessageCat castedObj = (TestMessageCat)obj;
            StaticNetSerializer_TestMessageCat.Serialize(castedObj, writer);
        }
        ,
        [typeof(TestMessageDog)] = (obj, writer) =>
        {
            TestMessageDog castedObj = (TestMessageDog)obj;
            StaticNetSerializer_TestMessageDog.Serialize(castedObj, writer);
        }
        ,
        [typeof(Unity.Mathematics.int2)] = (obj, writer) =>
        {
            Unity.Mathematics.int2 castedObj = (Unity.Mathematics.int2)obj;
            StaticNetSerializer_Unity_Mathematics_int2.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(UnityEngine.Vector2)] = (obj, writer) =>
        {
            UnityEngine.Vector2 castedObj = (UnityEngine.Vector2)obj;
            StaticNetSerializer_UnityEngine_Vector2.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(UnityEngine.Vector3)] = (obj, writer) =>
        {
            UnityEngine.Vector3 castedObj = (UnityEngine.Vector3)obj;
            StaticNetSerializer_UnityEngine_Vector3.Serialize(ref castedObj, writer);
        }
        ,
        [typeof(UnityEngine.Vector4)] = (obj, writer) =>
        {
            UnityEngine.Vector4 castedObj = (UnityEngine.Vector4)obj;
            StaticNetSerializer_UnityEngine_Vector4.Serialize(ref castedObj, writer);
        }
    };

    public static readonly Dictionary<UInt16, Func<BitStreamReader, object>> map_Deserialize = new Dictionary<UInt16, Func<BitStreamReader, object>>()
    {
        [0] = (reader) =>
        {
            CCC.Online.DataTransfer.NetMessageCancel obj = new CCC.Online.DataTransfer.NetMessageCancel();
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageCancel.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [1] = (reader) =>
        {
            CCC.Online.DataTransfer.NetMessagePacket obj = new CCC.Online.DataTransfer.NetMessagePacket();
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacket.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [2] = (reader) =>
        {
            CCC.Online.DataTransfer.NetMessagePacketACK obj = new CCC.Online.DataTransfer.NetMessagePacketACK();
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacketACK.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [3] = (reader) =>
        {
            CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader obj = new CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader();
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaManualPacketsHeader.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [4] = (reader) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamACK obj = new CCC.Online.DataTransfer.NetMessageViaStreamACK();
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamACK.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [5] = (reader) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader obj = new CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader();
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamChannelHeader.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [6] = (reader) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamReady obj = new CCC.Online.DataTransfer.NetMessageViaStreamReady();
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamReady.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [7] = (reader) =>
        {
            CCC.Online.DataTransfer.NetMessageViaStreamUpdate obj = new CCC.Online.DataTransfer.NetMessageViaStreamUpdate();
            StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamUpdate.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [8] = (reader) =>
        {
            fix obj = new fix();
            StaticNetSerializer_fix.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [9] = (reader) =>
        {
            fix2 obj = new fix2();
            StaticNetSerializer_fix2.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [10] = (reader) =>
        {
            fix2x2 obj = new fix2x2();
            StaticNetSerializer_fix2x2.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [11] = (reader) =>
        {
            fix2x3 obj = new fix2x3();
            StaticNetSerializer_fix2x3.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [12] = (reader) =>
        {
            fix3 obj = new fix3();
            StaticNetSerializer_fix3.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [13] = (reader) =>
        {
            fix3x2 obj = new fix3x2();
            StaticNetSerializer_fix3x2.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [14] = (reader) =>
        {
            fix3x3 obj = new fix3x3();
            StaticNetSerializer_fix3x3.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [15] = (reader) =>
        {
            fix4 obj = new fix4();
            StaticNetSerializer_fix4.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [16] = (reader) =>
        {
            fix4x4 obj = new fix4x4();
            StaticNetSerializer_fix4x4.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [17] = (reader) =>
        {
            fixQuaternion obj = new fixQuaternion();
            StaticNetSerializer_fixQuaternion.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [18] = (reader) =>
        {
            GameAction.UseParameters obj = new GameAction.UseParameters();
            StaticNetSerializer_GameAction_UseParameters.Deserialize(obj, reader);
            return obj;
        }
        ,
        [19] = (reader) =>
        {
            GameActionParameterBool.Data obj = new GameActionParameterBool.Data();
            StaticNetSerializer_GameActionParameterBool_Data.Deserialize(obj, reader);
            return obj;
        }
        ,
        [20] = (reader) =>
        {
            GameActionParameterEntity.Data obj = new GameActionParameterEntity.Data();
            StaticNetSerializer_GameActionParameterEntity_Data.Deserialize(obj, reader);
            return obj;
        }
        ,
        [21] = (reader) =>
        {
            GameActionParameterPosition.Data obj = new GameActionParameterPosition.Data();
            StaticNetSerializer_GameActionParameterPosition_Data.Deserialize(obj, reader);
            return obj;
        }
        ,
        [22] = (reader) =>
        {
            GameActionParameterSuccessRate.Data obj = new GameActionParameterSuccessRate.Data();
            StaticNetSerializer_GameActionParameterSuccessRate_Data.Deserialize(obj, reader);
            return obj;
        }
        ,
        [23] = (reader) =>
        {
            GameActionParameterTile.Data obj = new GameActionParameterTile.Data();
            StaticNetSerializer_GameActionParameterTile_Data.Deserialize(obj, reader);
            return obj;
        }
        ,
        [24] = (reader) =>
        {
            GameActionParameterVector.Data obj = new GameActionParameterVector.Data();
            StaticNetSerializer_GameActionParameterVector_Data.Deserialize(obj, reader);
            return obj;
        }
        ,
        [25] = (reader) =>
        {
            InputSubmissionId obj = new InputSubmissionId();
            StaticNetSerializer_InputSubmissionId.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [26] = (reader) =>
        {
            NetMessageAcceptSimSync obj = new NetMessageAcceptSimSync();
            StaticNetSerializer_NetMessageAcceptSimSync.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [27] = (reader) =>
        {
            NetMessageChatMessage obj = new NetMessageChatMessage();
            StaticNetSerializer_NetMessageChatMessage.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [28] = (reader) =>
        {
            NetMessageChatMessageSubmission obj = new NetMessageChatMessageSubmission();
            StaticNetSerializer_NetMessageChatMessageSubmission.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [29] = (reader) =>
        {
            NetMessageClientHello obj = new NetMessageClientHello();
            StaticNetSerializer_NetMessageClientHello.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [30] = (reader) =>
        {
            NetMessageDestroyValue obj = new NetMessageDestroyValue();
            StaticNetSerializer_NetMessageDestroyValue.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [31] = (reader) =>
        {
            NetMessageExample obj = new NetMessageExample();
            StaticNetSerializer_NetMessageExample.Deserialize(obj, reader);
            return obj;
        }
        ,
        [32] = (reader) =>
        {
            NetMessageInputSubmission obj = new NetMessageInputSubmission();
            StaticNetSerializer_NetMessageInputSubmission.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [33] = (reader) =>
        {
            NetMessagePlayerAssets obj = new NetMessagePlayerAssets();
            StaticNetSerializer_NetMessagePlayerAssets.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [34] = (reader) =>
        {
            NetMessagePlayerAssets.Data obj = new NetMessagePlayerAssets.Data();
            StaticNetSerializer_NetMessagePlayerAssets_Data.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [35] = (reader) =>
        {
            NetMessagePlayerIdAssignment obj = new NetMessagePlayerIdAssignment();
            StaticNetSerializer_NetMessagePlayerIdAssignment.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [36] = (reader) =>
        {
            NetMessagePlayerJoined obj = new NetMessagePlayerJoined();
            StaticNetSerializer_NetMessagePlayerJoined.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [37] = (reader) =>
        {
            NetMessagePlayerLeft obj = new NetMessagePlayerLeft();
            StaticNetSerializer_NetMessagePlayerLeft.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [38] = (reader) =>
        {
            NetMessagePlayerRepertoireSync obj = new NetMessagePlayerRepertoireSync();
            StaticNetSerializer_NetMessagePlayerRepertoireSync.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [39] = (reader) =>
        {
            NetMessageRequestSimSync obj = new NetMessageRequestSimSync();
            StaticNetSerializer_NetMessageRequestSimSync.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [40] = (reader) =>
        {
            NetMessageRequestValueSync obj = new NetMessageRequestValueSync();
            StaticNetSerializer_NetMessageRequestValueSync.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [41] = (reader) =>
        {
            NetMessageSerializedSimulation obj = new NetMessageSerializedSimulation();
            StaticNetSerializer_NetMessageSerializedSimulation.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [42] = (reader) =>
        {
            NetMessageSimPlayerIdAssignement obj = new NetMessageSimPlayerIdAssignement();
            StaticNetSerializer_NetMessageSimPlayerIdAssignement.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [43] = (reader) =>
        {
            NetMessageSimSyncFromFile obj = new NetMessageSimSyncFromFile();
            StaticNetSerializer_NetMessageSimSyncFromFile.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [44] = (reader) =>
        {
            NetMessageSyncValue obj = new NetMessageSyncValue();
            StaticNetSerializer_NetMessageSyncValue.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [45] = (reader) =>
        {
            NetMessageValueSyncComplete obj = new NetMessageValueSyncComplete();
            StaticNetSerializer_NetMessageValueSyncComplete.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [46] = (reader) =>
        {
            PersistentId obj = new PersistentId();
            StaticNetSerializer_PersistentId.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [47] = (reader) =>
        {
            PlayerId obj = new PlayerId();
            StaticNetSerializer_PlayerId.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [48] = (reader) =>
        {
            PlayerInfo obj = new PlayerInfo();
            StaticNetSerializer_PlayerInfo.Deserialize(obj, reader);
            return obj;
        }
        ,
        [49] = (reader) =>
        {
            Sim.Operations.SerializedWorld obj = new Sim.Operations.SerializedWorld();
            StaticNetSerializer_Sim_Operations_SerializedWorld.Deserialize(obj, reader);
            return obj;
        }
        ,
        [50] = (reader) =>
        {
            Sim.Operations.SerializedWorld.BlobAsset obj = new Sim.Operations.SerializedWorld.BlobAsset();
            StaticNetSerializer_Sim_Operations_SerializedWorld_BlobAsset.Deserialize(obj, reader);
            return obj;
        }
        ,
        [51] = (reader) =>
        {
            SimCommandLoadScene obj = new SimCommandLoadScene();
            StaticNetSerializer_SimCommandLoadScene.Deserialize(obj, reader);
            return obj;
        }
        ,
        [52] = (reader) =>
        {
            SimInputCheatAddAllItems obj = new SimInputCheatAddAllItems();
            StaticNetSerializer_SimInputCheatAddAllItems.Deserialize(obj, reader);
            return obj;
        }
        ,
        [53] = (reader) =>
        {
            SimInputCheatDamageSelf obj = new SimInputCheatDamageSelf();
            StaticNetSerializer_SimInputCheatDamageSelf.Deserialize(obj, reader);
            return obj;
        }
        ,
        [54] = (reader) =>
        {
            SimInputCheatImpulseSelf obj = new SimInputCheatImpulseSelf();
            StaticNetSerializer_SimInputCheatImpulseSelf.Deserialize(obj, reader);
            return obj;
        }
        ,
        [55] = (reader) =>
        {
            SimInputCheatRemoveAllCooldowns obj = new SimInputCheatRemoveAllCooldowns();
            StaticNetSerializer_SimInputCheatRemoveAllCooldowns.Deserialize(obj, reader);
            return obj;
        }
        ,
        [56] = (reader) =>
        {
            SimInputCheatTeleport obj = new SimInputCheatTeleport();
            StaticNetSerializer_SimInputCheatTeleport.Deserialize(obj, reader);
            return obj;
        }
        ,
        [57] = (reader) =>
        {
            SimInputCheatToggleInvincible obj = new SimInputCheatToggleInvincible();
            StaticNetSerializer_SimInputCheatToggleInvincible.Deserialize(obj, reader);
            return obj;
        }
        ,
        [58] = (reader) =>
        {
            SimInputPlayerCreate obj = new SimInputPlayerCreate();
            StaticNetSerializer_SimInputPlayerCreate.Deserialize(obj, reader);
            return obj;
        }
        ,
        [59] = (reader) =>
        {
            SimInputSetPlayerActive obj = new SimInputSetPlayerActive();
            StaticNetSerializer_SimInputSetPlayerActive.Deserialize(obj, reader);
            return obj;
        }
        ,
        [60] = (reader) =>
        {
            SimInputSubmission obj = new SimInputSubmission();
            StaticNetSerializer_SimInputSubmission.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [61] = (reader) =>
        {
            SimPlayerInputClickSignalEmitter obj = new SimPlayerInputClickSignalEmitter();
            StaticNetSerializer_SimPlayerInputClickSignalEmitter.Deserialize(obj, reader);
            return obj;
        }
        ,
        [62] = (reader) =>
        {
            SimPlayerInputDropItem obj = new SimPlayerInputDropItem();
            StaticNetSerializer_SimPlayerInputDropItem.Deserialize(obj, reader);
            return obj;
        }
        ,
        [63] = (reader) =>
        {
            SimPlayerInputEquipItem obj = new SimPlayerInputEquipItem();
            StaticNetSerializer_SimPlayerInputEquipItem.Deserialize(obj, reader);
            return obj;
        }
        ,
        [64] = (reader) =>
        {
            SimPlayerInputJump obj = new SimPlayerInputJump();
            StaticNetSerializer_SimPlayerInputJump.Deserialize(obj, reader);
            return obj;
        }
        ,
        [65] = (reader) =>
        {
            SimPlayerInputMove obj = new SimPlayerInputMove();
            StaticNetSerializer_SimPlayerInputMove.Deserialize(obj, reader);
            return obj;
        }
        ,
        [66] = (reader) =>
        {
            SimPlayerInputNextTurn obj = new SimPlayerInputNextTurn();
            StaticNetSerializer_SimPlayerInputNextTurn.Deserialize(obj, reader);
            return obj;
        }
        ,
        [67] = (reader) =>
        {
            SimPlayerInputSelectStartingInventory obj = new SimPlayerInputSelectStartingInventory();
            StaticNetSerializer_SimPlayerInputSelectStartingInventory.Deserialize(obj, reader);
            return obj;
        }
        ,
        [68] = (reader) =>
        {
            SimPlayerInputSetPawnDoodle obj = new SimPlayerInputSetPawnDoodle();
            StaticNetSerializer_SimPlayerInputSetPawnDoodle.Deserialize(obj, reader);
            return obj;
        }
        ,
        [69] = (reader) =>
        {
            SimPlayerInputSetPawnName obj = new SimPlayerInputSetPawnName();
            StaticNetSerializer_SimPlayerInputSetPawnName.Deserialize(obj, reader);
            return obj;
        }
        ,
        [70] = (reader) =>
        {
            SimPlayerInputUseItem obj = new SimPlayerInputUseItem();
            StaticNetSerializer_SimPlayerInputUseItem.Deserialize(obj, reader);
            return obj;
        }
        ,
        [71] = (reader) =>
        {
            SimPlayerInputUseObjectGameAction obj = new SimPlayerInputUseObjectGameAction();
            StaticNetSerializer_SimPlayerInputUseObjectGameAction.Deserialize(obj, reader);
            return obj;
        }
        ,
        [72] = (reader) =>
        {
            SimulationControl.NetMessageSimTick obj = new SimulationControl.NetMessageSimTick();
            StaticNetSerializer_SimulationControl_NetMessageSimTick.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [73] = (reader) =>
        {
            SimulationControl.SimTickData obj = new SimulationControl.SimTickData();
            StaticNetSerializer_SimulationControl_SimTickData.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [74] = (reader) =>
        {
            SyncedValueCurrentLevel obj = new SyncedValueCurrentLevel();
            StaticNetSerializer_SyncedValueCurrentLevel.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [75] = (reader) =>
        {
            TestMessage obj = new TestMessage();
            StaticNetSerializer_TestMessage.Deserialize(obj, reader);
            return obj;
        }
        ,
        [76] = (reader) =>
        {
            TestMessageAnimal obj = new TestMessageAnimal();
            StaticNetSerializer_TestMessageAnimal.Deserialize(obj, reader);
            return obj;
        }
        ,
        [77] = (reader) =>
        {
            TestMessageCat obj = new TestMessageCat();
            StaticNetSerializer_TestMessageCat.Deserialize(obj, reader);
            return obj;
        }
        ,
        [78] = (reader) =>
        {
            TestMessageDog obj = new TestMessageDog();
            StaticNetSerializer_TestMessageDog.Deserialize(obj, reader);
            return obj;
        }
        ,
        [79] = (reader) =>
        {
            Unity.Mathematics.int2 obj = new Unity.Mathematics.int2();
            StaticNetSerializer_Unity_Mathematics_int2.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [80] = (reader) =>
        {
            UnityEngine.Vector2 obj = new UnityEngine.Vector2();
            StaticNetSerializer_UnityEngine_Vector2.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [81] = (reader) =>
        {
            UnityEngine.Vector3 obj = new UnityEngine.Vector3();
            StaticNetSerializer_UnityEngine_Vector3.Deserialize(ref obj, reader);
            return obj;
        }
        ,
        [82] = (reader) =>
        {
            UnityEngine.Vector4 obj = new UnityEngine.Vector4();
            StaticNetSerializer_UnityEngine_Vector4.Deserialize(ref obj, reader);
            return obj;
        }
    };
}
