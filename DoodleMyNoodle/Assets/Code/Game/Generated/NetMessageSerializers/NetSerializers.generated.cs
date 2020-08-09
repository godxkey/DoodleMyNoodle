// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageChatMessage
{
    public static int GetNetBitSize(ref NetMessageChatMessage obj)
    {
        int result = 0;
        result += StaticNetSerializer_PlayerId.GetNetBitSize(ref obj.playerId);
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.message);
        return result;
    }

    public static void NetSerialize(ref NetMessageChatMessage obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PlayerId.NetSerialize(ref obj.playerId, writer);
        StaticNetSerializer_System_String.NetSerialize(ref obj.message, writer);
    }

    public static void NetDeserialize(ref NetMessageChatMessage obj, BitStreamReader reader)
    {
        StaticNetSerializer_PlayerId.NetDeserialize(ref obj.playerId, reader);
        StaticNetSerializer_System_String.NetDeserialize(ref obj.message, reader);
    }
}
public static class StaticNetSerializer_NetMessageChatMessageSubmission
{
    public static int GetNetBitSize(ref NetMessageChatMessageSubmission obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.message);
        return result;
    }

    public static void NetSerialize(ref NetMessageChatMessageSubmission obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.message, writer);
    }

    public static void NetDeserialize(ref NetMessageChatMessageSubmission obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.message, reader);
    }
}
public static class StaticNetSerializer_NetMessageClientHello
{
    public static int GetNetBitSize(ref NetMessageClientHello obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.playerName);
        return result;
    }

    public static void NetSerialize(ref NetMessageClientHello obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.playerName, writer);
    }

    public static void NetDeserialize(ref NetMessageClientHello obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.playerName, reader);
    }
}
public static class StaticNetSerializer_NetMessageExample
{
    public static int GetNetBitSize_Class(NetMessageExample obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(NetMessageExample obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.valueString);
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.valueInt);
        result += StaticNetSerializer_System_UInt32.GetNetBitSize(ref obj.valueUInt);
        result += StaticNetSerializer_System_Int16.GetNetBitSize(ref obj.valueShort);
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.valueUShort);
        result += StaticNetSerializer_System_Boolean.GetNetBitSize(ref obj.valueBool);
        result += StaticNetSerializer_System_Byte.GetNetBitSize(ref obj.valueByte);
        result += ArrayNetSerializer_System_Int32.GetNetBitSize(ref obj.listOnInts);
        return result;
    }

    public static void NetSerialize_Class(NetMessageExample obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(NetMessageExample obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.valueString, writer);
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.valueInt, writer);
        StaticNetSerializer_System_UInt32.NetSerialize(ref obj.valueUInt, writer);
        StaticNetSerializer_System_Int16.NetSerialize(ref obj.valueShort, writer);
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.valueUShort, writer);
        StaticNetSerializer_System_Boolean.NetSerialize(ref obj.valueBool, writer);
        StaticNetSerializer_System_Byte.NetSerialize(ref obj.valueByte, writer);
        ArrayNetSerializer_System_Int32.NetSerialize(ref obj.listOnInts, writer);
    }

    public static NetMessageExample NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        NetMessageExample obj = new NetMessageExample();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(NetMessageExample obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.valueString, reader);
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.valueInt, reader);
        StaticNetSerializer_System_UInt32.NetDeserialize(ref obj.valueUInt, reader);
        StaticNetSerializer_System_Int16.NetDeserialize(ref obj.valueShort, reader);
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.valueUShort, reader);
        StaticNetSerializer_System_Boolean.NetDeserialize(ref obj.valueBool, reader);
        StaticNetSerializer_System_Byte.NetDeserialize(ref obj.valueByte, reader);
        ArrayNetSerializer_System_Int32.NetDeserialize(ref obj.listOnInts, reader);
    }
}
public static class StaticNetSerializer_NetMessagePlayerAssets
{
    public static int GetNetBitSize(ref NetMessagePlayerAssets obj)
    {
        int result = 0;
        result += ArrayNetSerializer_NetMessagePlayerAssets_Data.GetNetBitSize(ref obj.Assets);
        return result;
    }

    public static void NetSerialize(ref NetMessagePlayerAssets obj, BitStreamWriter writer)
    {
        ArrayNetSerializer_NetMessagePlayerAssets_Data.NetSerialize(ref obj.Assets, writer);
    }

    public static void NetDeserialize(ref NetMessagePlayerAssets obj, BitStreamReader reader)
    {
        ArrayNetSerializer_NetMessagePlayerAssets_Data.NetDeserialize(ref obj.Assets, reader);
    }
}
public static class StaticNetSerializer_NetMessagePlayerAssets_Data
{
    public static int GetNetBitSize(ref NetMessagePlayerAssets.Data obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Guid.GetNetBitSize(ref obj.Guid);
        result += StaticNetSerializer_System_Byte.GetNetBitSize();
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.Author);
        result += StaticNetSerializer_System_DateTime.GetNetBitSize(ref obj.UtcCreationTime);
        result += ArrayNetSerializer_System_Byte.GetNetBitSize(ref obj.AssetData);
        return result;
    }

    public static void NetSerialize(ref NetMessagePlayerAssets.Data obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Guid.NetSerialize(ref obj.Guid, writer);
        StaticNetSerializer_System_Byte.NetSerialize((System.Byte)obj.Type, writer);
        StaticNetSerializer_System_String.NetSerialize(ref obj.Author, writer);
        StaticNetSerializer_System_DateTime.NetSerialize(ref obj.UtcCreationTime, writer);
        ArrayNetSerializer_System_Byte.NetSerialize(ref obj.AssetData, writer);
    }

    public static void NetDeserialize(ref NetMessagePlayerAssets.Data obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Guid.NetDeserialize(ref obj.Guid, reader);
        obj.Type = (PlayerAssetType)StaticNetSerializer_System_Byte.NetDeserialize(reader);
        StaticNetSerializer_System_String.NetDeserialize(ref obj.Author, reader);
        StaticNetSerializer_System_DateTime.NetDeserialize(ref obj.UtcCreationTime, reader);
        ArrayNetSerializer_System_Byte.NetDeserialize(ref obj.AssetData, reader);
    }
}
public static class StaticNetSerializer_NetMessagePlayerIdAssignment
{
    public static int GetNetBitSize(ref NetMessagePlayerIdAssignment obj)
    {
        int result = 0;
        result += StaticNetSerializer_PlayerId.GetNetBitSize(ref obj.playerId);
        return result;
    }

    public static void NetSerialize(ref NetMessagePlayerIdAssignment obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PlayerId.NetSerialize(ref obj.playerId, writer);
    }

    public static void NetDeserialize(ref NetMessagePlayerIdAssignment obj, BitStreamReader reader)
    {
        StaticNetSerializer_PlayerId.NetDeserialize(ref obj.playerId, reader);
    }
}
public static class StaticNetSerializer_NetMessagePlayerJoined
{
    public static int GetNetBitSize(ref NetMessagePlayerJoined obj)
    {
        int result = 0;
        result += StaticNetSerializer_PlayerInfo.GetNetBitSize_Class(obj.playerInfo);
        return result;
    }

    public static void NetSerialize(ref NetMessagePlayerJoined obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PlayerInfo.NetSerialize_Class(obj.playerInfo, writer);
    }

    public static void NetDeserialize(ref NetMessagePlayerJoined obj, BitStreamReader reader)
    {
        obj.playerInfo = StaticNetSerializer_PlayerInfo.NetDeserialize_Class(reader);
    }
}
public static class StaticNetSerializer_NetMessagePlayerLeft
{
    public static int GetNetBitSize(ref NetMessagePlayerLeft obj)
    {
        int result = 0;
        result += StaticNetSerializer_PlayerId.GetNetBitSize(ref obj.playerId);
        return result;
    }

    public static void NetSerialize(ref NetMessagePlayerLeft obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PlayerId.NetSerialize(ref obj.playerId, writer);
    }

    public static void NetDeserialize(ref NetMessagePlayerLeft obj, BitStreamReader reader)
    {
        StaticNetSerializer_PlayerId.NetDeserialize(ref obj.playerId, reader);
    }
}
public static class StaticNetSerializer_NetMessagePlayerRepertoireSync
{
    public static int GetNetBitSize(ref NetMessagePlayerRepertoireSync obj)
    {
        int result = 0;
        result += ArrayNetSerializer_PlayerInfo.GetNetBitSize(ref obj.players);
        return result;
    }

    public static void NetSerialize(ref NetMessagePlayerRepertoireSync obj, BitStreamWriter writer)
    {
        ArrayNetSerializer_PlayerInfo.NetSerialize(ref obj.players, writer);
    }

    public static void NetDeserialize(ref NetMessagePlayerRepertoireSync obj, BitStreamReader reader)
    {
        ArrayNetSerializer_PlayerInfo.NetDeserialize(ref obj.players, reader);
    }
}
public static class StaticNetSerializer_NetMessageSimPlayerIdAssignement
{
    public static int GetNetBitSize(ref NetMessageSimPlayerIdAssignement obj)
    {
        int result = 0;
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_PlayerId.GetNetBitSize(ref obj.PlayerId);
        return result;
    }

    public static void NetSerialize(ref NetMessageSimPlayerIdAssignement obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_PlayerId.NetSerialize(ref obj.PlayerId, writer);
    }

    public static void NetDeserialize(ref NetMessageSimPlayerIdAssignement obj, BitStreamReader reader)
    {
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_PlayerId.NetDeserialize(ref obj.PlayerId, reader);
    }
}
public static class StaticNetSerializer_PlayerId
{
    public static int GetNetBitSize(ref PlayerId obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.Value);
        return result;
    }

    public static void NetSerialize(ref PlayerId obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.Value, writer);
    }

    public static void NetDeserialize(ref PlayerId obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.Value, reader);
    }
}
public static class StaticNetSerializer_PlayerInfo
{
    public static int GetNetBitSize_Class(PlayerInfo obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(PlayerInfo obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.PlayerName);
        result += StaticNetSerializer_PlayerId.GetNetBitSize(ref obj.PlayerId);
        result += StaticNetSerializer_System_Boolean.GetNetBitSize(ref obj.IsMaster);
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.SimPlayerId);
        return result;
    }

    public static void NetSerialize_Class(PlayerInfo obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(PlayerInfo obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.PlayerName, writer);
        StaticNetSerializer_PlayerId.NetSerialize(ref obj.PlayerId, writer);
        StaticNetSerializer_System_Boolean.NetSerialize(ref obj.IsMaster, writer);
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.SimPlayerId, writer);
    }

    public static PlayerInfo NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        PlayerInfo obj = new PlayerInfo();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(PlayerInfo obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.PlayerName, reader);
        StaticNetSerializer_PlayerId.NetDeserialize(ref obj.PlayerId, reader);
        StaticNetSerializer_System_Boolean.NetDeserialize(ref obj.IsMaster, reader);
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.SimPlayerId, reader);
    }
}
public static class StaticNetSerializer_SyncedValueCurrentLevel
{
    public static int GetNetBitSize(ref SyncedValueCurrentLevel obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.Name);
        return result;
    }

    public static void NetSerialize(ref SyncedValueCurrentLevel obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.Name, writer);
    }

    public static void NetDeserialize(ref SyncedValueCurrentLevel obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.Name, reader);
    }
}
public static class StaticNetSerializer_TestMessage
{
    public static int GetNetBitSize_Class(TestMessage obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(TestMessage obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.valueString);
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.valueInt);
        result += StaticNetSerializer_System_UInt32.GetNetBitSize(ref obj.valueUInt);
        result += StaticNetSerializer_System_Int16.GetNetBitSize(ref obj.valueShort);
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.valueUShort);
        result += StaticNetSerializer_System_Boolean.GetNetBitSize(ref obj.valueBool);
        result += StaticNetSerializer_System_Byte.GetNetBitSize(ref obj.valueByte);
        result += ArrayNetSerializer_System_Int32.GetNetBitSize(ref obj.arrayOfInts);
        result += ListNetSerializer_System_Int32.GetNetBitSize_Class(obj.listOfInts);
        result += StaticNetSerializer_TestMessageCat.GetNetBitSize_Class(obj.cat);
        result += StaticNetSerializer_TestMessageDog.GetNetBitSize_Class(obj.dog);
        result += StaticNetSerializer_TestMessageAnimal.GetNetBitSize_Class(obj.animal);
        result += ArrayNetSerializer_TestMessageAnimal.GetNetBitSize(ref obj.animals);
        return result;
    }

    public static void NetSerialize_Class(TestMessage obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(TestMessage obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.valueString, writer);
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.valueInt, writer);
        StaticNetSerializer_System_UInt32.NetSerialize(ref obj.valueUInt, writer);
        StaticNetSerializer_System_Int16.NetSerialize(ref obj.valueShort, writer);
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.valueUShort, writer);
        StaticNetSerializer_System_Boolean.NetSerialize(ref obj.valueBool, writer);
        StaticNetSerializer_System_Byte.NetSerialize(ref obj.valueByte, writer);
        ArrayNetSerializer_System_Int32.NetSerialize(ref obj.arrayOfInts, writer);
        ListNetSerializer_System_Int32.NetSerialize_Class(obj.listOfInts, writer);
        StaticNetSerializer_TestMessageCat.NetSerialize_Class(obj.cat, writer);
        StaticNetSerializer_TestMessageDog.NetSerialize_Class(obj.dog, writer);
        StaticNetSerializer_TestMessageAnimal.NetSerialize_Class(obj.animal, writer);
        ArrayNetSerializer_TestMessageAnimal.NetSerialize(ref obj.animals, writer);
    }

    public static TestMessage NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        TestMessage obj = new TestMessage();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(TestMessage obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.valueString, reader);
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.valueInt, reader);
        StaticNetSerializer_System_UInt32.NetDeserialize(ref obj.valueUInt, reader);
        StaticNetSerializer_System_Int16.NetDeserialize(ref obj.valueShort, reader);
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.valueUShort, reader);
        StaticNetSerializer_System_Boolean.NetDeserialize(ref obj.valueBool, reader);
        StaticNetSerializer_System_Byte.NetDeserialize(ref obj.valueByte, reader);
        ArrayNetSerializer_System_Int32.NetDeserialize(ref obj.arrayOfInts, reader);
        obj.listOfInts = ListNetSerializer_System_Int32.NetDeserialize_Class(reader);
        obj.cat = StaticNetSerializer_TestMessageCat.NetDeserialize_Class(reader);
        obj.dog = StaticNetSerializer_TestMessageDog.NetDeserialize_Class(reader);
        obj.animal = StaticNetSerializer_TestMessageAnimal.NetDeserialize_Class(reader);
        ArrayNetSerializer_TestMessageAnimal.NetDeserialize(ref obj.animals, reader);
    }
}
public static class StaticNetSerializer_TestMessageAnimal
{
    public static int GetNetBitSize_Class(TestMessageAnimal obj)
    {
        if (obj == null)
            return 1;
        return 1 + DynamicNetSerializer.GetNetBitSize(obj);
    }

    public static int GetNetBitSize(TestMessageAnimal obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.name);
        return result;
    }

    public static void NetSerialize_Class(TestMessageAnimal obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        DynamicNetSerializer.NetSerialize(obj, writer);
    }
    public static void NetSerialize(TestMessageAnimal obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.name, writer);
    }

    public static TestMessageAnimal NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (TestMessageAnimal)DynamicNetSerializer.NetDeserialize(reader);
    }
    public static void NetDeserialize(TestMessageAnimal obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.name, reader);
    }
}
public static class StaticNetSerializer_TestMessageCat
{
    public static int GetNetBitSize_Class(TestMessageCat obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(TestMessageCat obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.numberOfLivesLeft);
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.name);
        result += StaticNetSerializer_TestMessageAnimal.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(TestMessageCat obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(TestMessageCat obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.numberOfLivesLeft, writer);
        StaticNetSerializer_System_String.NetSerialize(ref obj.name, writer);
        StaticNetSerializer_TestMessageAnimal.NetSerialize(obj, writer);
    }

    public static TestMessageCat NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        TestMessageCat obj = new TestMessageCat();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(TestMessageCat obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.numberOfLivesLeft, reader);
        StaticNetSerializer_System_String.NetDeserialize(ref obj.name, reader);
        StaticNetSerializer_TestMessageAnimal.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_TestMessageDog
{
    public static int GetNetBitSize_Class(TestMessageDog obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(TestMessageDog obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Boolean.GetNetBitSize(ref obj.isAGoodBoy);
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.name);
        result += StaticNetSerializer_TestMessageAnimal.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(TestMessageDog obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(TestMessageDog obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Boolean.NetSerialize(ref obj.isAGoodBoy, writer);
        StaticNetSerializer_System_String.NetSerialize(ref obj.name, writer);
        StaticNetSerializer_TestMessageAnimal.NetSerialize(obj, writer);
    }

    public static TestMessageDog NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        TestMessageDog obj = new TestMessageDog();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(TestMessageDog obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Boolean.NetDeserialize(ref obj.isAGoodBoy, reader);
        StaticNetSerializer_System_String.NetDeserialize(ref obj.name, reader);
        StaticNetSerializer_TestMessageAnimal.NetDeserialize(obj, reader);
    }
}

public static class ArrayNetSerializer_NetMessagePlayerAssets_Data
{
    public static int GetNetBitSize(ref NetMessagePlayerAssets.Data[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_NetMessagePlayerAssets_Data.GetNetBitSize(ref obj[i]);
        }
        return result;
    }

    public static void NetSerialize(ref NetMessagePlayerAssets.Data[] obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteInt32(obj.Length);
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_NetMessagePlayerAssets_Data.NetSerialize(ref obj[i], writer);
        }
    }

    public static void NetDeserialize(ref NetMessagePlayerAssets.Data[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new NetMessagePlayerAssets.Data[reader.ReadInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_NetMessagePlayerAssets_Data.NetDeserialize(ref obj[i], reader);
        }
    }
}

public static class ArrayNetSerializer_PlayerInfo
{
    public static int GetNetBitSize(ref PlayerInfo[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_PlayerInfo.GetNetBitSize_Class(obj[i]);
        }
        return result;
    }

    public static void NetSerialize(ref PlayerInfo[] obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteInt32(obj.Length);
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_PlayerInfo.NetSerialize_Class(obj[i], writer);
        }
    }

    public static void NetDeserialize(ref PlayerInfo[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new PlayerInfo[reader.ReadInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i] = StaticNetSerializer_PlayerInfo.NetDeserialize_Class(reader);
        }
    }
}

public static class ArrayNetSerializer_TestMessageAnimal
{
    public static int GetNetBitSize(ref TestMessageAnimal[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_TestMessageAnimal.GetNetBitSize_Class(obj[i]);
        }
        return result;
    }

    public static void NetSerialize(ref TestMessageAnimal[] obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteInt32(obj.Length);
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_TestMessageAnimal.NetSerialize_Class(obj[i], writer);
        }
    }

    public static void NetDeserialize(ref TestMessageAnimal[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new TestMessageAnimal[reader.ReadInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i] = StaticNetSerializer_TestMessageAnimal.NetDeserialize_Class(reader);
        }
    }
}
