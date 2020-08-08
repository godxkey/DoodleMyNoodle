// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_GameAction_ParameterData
{
    public static int GetNetBitSize_Class(GameAction.ParameterData obj)
    {
        if (obj == null)
            return 1;
        return 1 + DynamicNetSerializer.GetNetBitSize(obj);
    }

    public static int GetNetBitSize(GameAction.ParameterData obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Byte.GetNetBitSize(ref obj.ParamIndex);
        return result;
    }

    public static void NetSerialize_Class(GameAction.ParameterData obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        DynamicNetSerializer.NetSerialize(obj, writer);
    }
    public static void NetSerialize(GameAction.ParameterData obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Byte.NetSerialize(ref obj.ParamIndex, writer);
    }

    public static GameAction.ParameterData NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (GameAction.ParameterData)DynamicNetSerializer.NetDeserialize(reader);
    }
    public static void NetDeserialize(GameAction.ParameterData obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Byte.NetDeserialize(ref obj.ParamIndex, reader);
    }
}
public static class StaticNetSerializer_GameAction_UseParameters
{
    public static int GetNetBitSize_Class(GameAction.UseParameters obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(GameAction.UseParameters obj)
    {
        int result = 0;
        result += ArrayNetSerializer_GameAction_ParameterData.GetNetBitSize(ref obj.ParameterDatas);
        return result;
    }

    public static void NetSerialize_Class(GameAction.UseParameters obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(GameAction.UseParameters obj, BitStreamWriter writer)
    {
        ArrayNetSerializer_GameAction_ParameterData.NetSerialize(ref obj.ParameterDatas, writer);
    }

    public static GameAction.UseParameters NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        GameAction.UseParameters obj = new GameAction.UseParameters();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(GameAction.UseParameters obj, BitStreamReader reader)
    {
        ArrayNetSerializer_GameAction_ParameterData.NetDeserialize(ref obj.ParameterDatas, reader);
    }
}
public static class StaticNetSerializer_GameActionParameterSelfTarget_Data
{
    public static int GetNetBitSize_Class(GameActionParameterSelfTarget.Data obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(GameActionParameterSelfTarget.Data obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Byte.GetNetBitSize(ref obj.ParamIndex);
        result += StaticNetSerializer_GameAction_ParameterData.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(GameActionParameterSelfTarget.Data obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(GameActionParameterSelfTarget.Data obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Byte.NetSerialize(ref obj.ParamIndex, writer);
        StaticNetSerializer_GameAction_ParameterData.NetSerialize(obj, writer);
    }

    public static GameActionParameterSelfTarget.Data NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        GameActionParameterSelfTarget.Data obj = new GameActionParameterSelfTarget.Data();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(GameActionParameterSelfTarget.Data obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Byte.NetDeserialize(ref obj.ParamIndex, reader);
        StaticNetSerializer_GameAction_ParameterData.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_GameActionParameterTile_Data
{
    public static int GetNetBitSize_Class(GameActionParameterTile.Data obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(GameActionParameterTile.Data obj)
    {
        int result = 0;
        result += StaticNetSerializer_Unity_Mathematics_int2.GetNetBitSize(ref obj.Tile);
        result += StaticNetSerializer_System_Byte.GetNetBitSize(ref obj.ParamIndex);
        result += StaticNetSerializer_GameAction_ParameterData.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(GameActionParameterTile.Data obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(GameActionParameterTile.Data obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Unity_Mathematics_int2.NetSerialize(ref obj.Tile, writer);
        StaticNetSerializer_System_Byte.NetSerialize(ref obj.ParamIndex, writer);
        StaticNetSerializer_GameAction_ParameterData.NetSerialize(obj, writer);
    }

    public static GameActionParameterTile.Data NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        GameActionParameterTile.Data obj = new GameActionParameterTile.Data();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(GameActionParameterTile.Data obj, BitStreamReader reader)
    {
        StaticNetSerializer_Unity_Mathematics_int2.NetDeserialize(ref obj.Tile, reader);
        StaticNetSerializer_System_Byte.NetDeserialize(ref obj.ParamIndex, reader);
        StaticNetSerializer_GameAction_ParameterData.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimInputCheatKillPlayerPawn
{
    public static int GetNetBitSize_Class(SimInputCheatKillPlayerPawn obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimInputCheatKillPlayerPawn obj)
    {
        int result = 0;
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.PlayerId);
        result += StaticNetSerializer_SimCheatInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimInputCheatKillPlayerPawn obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimInputCheatKillPlayerPawn obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.PlayerId, writer);
        StaticNetSerializer_SimCheatInput.NetSerialize(obj, writer);
    }

    public static SimInputCheatKillPlayerPawn NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputCheatKillPlayerPawn obj = new SimInputCheatKillPlayerPawn();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimInputCheatKillPlayerPawn obj, BitStreamReader reader)
    {
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.PlayerId, reader);
        StaticNetSerializer_SimCheatInput.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimInputPlayerCreate
{
    public static int GetNetBitSize_Class(SimInputPlayerCreate obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimInputPlayerCreate obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.PlayerName);
        result += StaticNetSerializer_SimMasterInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimInputPlayerCreate obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimInputPlayerCreate obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.PlayerName, writer);
        StaticNetSerializer_SimMasterInput.NetSerialize(obj, writer);
    }

    public static SimInputPlayerCreate NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputPlayerCreate obj = new SimInputPlayerCreate();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimInputPlayerCreate obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.PlayerName, reader);
        StaticNetSerializer_SimMasterInput.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInput
{
    public static int GetNetBitSize_Class(SimPlayerInput obj)
    {
        if (obj == null)
            return 1;
        return 1 + DynamicNetSerializer.GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimPlayerInput obj)
    {
        int result = 0;
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimPlayerInput obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        DynamicNetSerializer.NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimPlayerInput obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimInput.NetSerialize(obj, writer);
    }

    public static SimPlayerInput NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimPlayerInput)DynamicNetSerializer.NetDeserialize(reader);
    }
    public static void NetDeserialize(SimPlayerInput obj, BitStreamReader reader)
    {
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimInput.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputDropItem
{
    public static int GetNetBitSize_Class(SimPlayerInputDropItem obj)
    {
        if (obj == null)
            return 1;
        return 1 + DynamicNetSerializer.GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimPlayerInputDropItem obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.ItemIndex);
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimPlayerInputDropItem obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        DynamicNetSerializer.NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimPlayerInputDropItem obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.ItemIndex, writer);
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.NetSerialize(obj, writer);
    }

    public static SimPlayerInputDropItem NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimPlayerInputDropItem)DynamicNetSerializer.NetDeserialize(reader);
    }
    public static void NetDeserialize(SimPlayerInputDropItem obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.ItemIndex, reader);
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputEquipItem
{
    public static int GetNetBitSize_Class(SimPlayerInputEquipItem obj)
    {
        if (obj == null)
            return 1;
        return 1 + DynamicNetSerializer.GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimPlayerInputEquipItem obj)
    {
        int result = 0;
        result += StaticNetSerializer_Unity_Mathematics_int2.GetNetBitSize(ref obj.ItemEntityPosition);
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.ItemIndex);
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimPlayerInputEquipItem obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        DynamicNetSerializer.NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimPlayerInputEquipItem obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Unity_Mathematics_int2.NetSerialize(ref obj.ItemEntityPosition, writer);
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.ItemIndex, writer);
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.NetSerialize(obj, writer);
    }

    public static SimPlayerInputEquipItem NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimPlayerInputEquipItem)DynamicNetSerializer.NetDeserialize(reader);
    }
    public static void NetDeserialize(SimPlayerInputEquipItem obj, BitStreamReader reader)
    {
        StaticNetSerializer_Unity_Mathematics_int2.NetDeserialize(ref obj.ItemEntityPosition, reader);
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.ItemIndex, reader);
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputNextTurn
{
    public static int GetNetBitSize_Class(SimPlayerInputNextTurn obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimPlayerInputNextTurn obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Boolean.GetNetBitSize(ref obj.ReadyForNextTurn);
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimPlayerInputNextTurn obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimPlayerInputNextTurn obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Boolean.NetSerialize(ref obj.ReadyForNextTurn, writer);
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.NetSerialize(obj, writer);
    }

    public static SimPlayerInputNextTurn NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInputNextTurn obj = new SimPlayerInputNextTurn();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimPlayerInputNextTurn obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Boolean.NetDeserialize(ref obj.ReadyForNextTurn, reader);
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputSelectStartingInventory
{
    public static int GetNetBitSize_Class(SimPlayerInputSelectStartingInventory obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimPlayerInputSelectStartingInventory obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.KitNumber);
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimPlayerInputSelectStartingInventory obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimPlayerInputSelectStartingInventory obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.KitNumber, writer);
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.NetSerialize(obj, writer);
    }

    public static SimPlayerInputSelectStartingInventory NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInputSelectStartingInventory obj = new SimPlayerInputSelectStartingInventory();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimPlayerInputSelectStartingInventory obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.KitNumber, reader);
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputSetPawnDoodle
{
    public static int GetNetBitSize_Class(SimPlayerInputSetPawnDoodle obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimPlayerInputSetPawnDoodle obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Guid.GetNetBitSize(ref obj.DoodleId);
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimPlayerInputSetPawnDoodle obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimPlayerInputSetPawnDoodle obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Guid.NetSerialize(ref obj.DoodleId, writer);
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.NetSerialize(obj, writer);
    }

    public static SimPlayerInputSetPawnDoodle NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInputSetPawnDoodle obj = new SimPlayerInputSetPawnDoodle();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimPlayerInputSetPawnDoodle obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Guid.NetDeserialize(ref obj.DoodleId, reader);
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputSetPawnName
{
    public static int GetNetBitSize_Class(SimPlayerInputSetPawnName obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimPlayerInputSetPawnName obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.Name);
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimPlayerInputSetPawnName obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimPlayerInputSetPawnName obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.NetSerialize(ref obj.Name, writer);
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.NetSerialize(obj, writer);
    }

    public static SimPlayerInputSetPawnName NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInputSetPawnName obj = new SimPlayerInputSetPawnName();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimPlayerInputSetPawnName obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.NetDeserialize(ref obj.Name, reader);
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputSetStartingInventory
{
    public static int GetNetBitSize_Class(SimPlayerInputSetStartingInventory obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimPlayerInputSetStartingInventory obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.KitNumber);
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimPlayerInputSetStartingInventory obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimPlayerInputSetStartingInventory obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.KitNumber, writer);
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.NetSerialize(obj, writer);
    }

    public static SimPlayerInputSetStartingInventory NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInputSetStartingInventory obj = new SimPlayerInputSetStartingInventory();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimPlayerInputSetStartingInventory obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.KitNumber, reader);
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputUseInteractable
{
    public static int GetNetBitSize_Class(SimPlayerInputUseInteractable obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimPlayerInputUseInteractable obj)
    {
        int result = 0;
        result += StaticNetSerializer_Unity_Mathematics_int2.GetNetBitSize(ref obj.InteractablePosition);
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimPlayerInputUseInteractable obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimPlayerInputUseInteractable obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Unity_Mathematics_int2.NetSerialize(ref obj.InteractablePosition, writer);
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.NetSerialize(obj, writer);
    }

    public static SimPlayerInputUseInteractable NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInputUseInteractable obj = new SimPlayerInputUseInteractable();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimPlayerInputUseInteractable obj, BitStreamReader reader)
    {
        StaticNetSerializer_Unity_Mathematics_int2.NetDeserialize(ref obj.InteractablePosition, reader);
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.NetDeserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputUseItem
{
    public static int GetNetBitSize_Class(SimPlayerInputUseItem obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetNetBitSize(obj);
    }

    public static int GetNetBitSize(SimPlayerInputUseItem obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.ItemIndex);
        result += StaticNetSerializer_GameAction_UseParameters.GetNetBitSize_Class(obj.UseData);
        result += StaticNetSerializer_PersistentId.GetNetBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetNetBitSize(obj);
        return result;
    }

    public static void NetSerialize_Class(SimPlayerInputUseItem obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerialize(obj, writer);
    }
    public static void NetSerialize(SimPlayerInputUseItem obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.ItemIndex, writer);
        StaticNetSerializer_GameAction_UseParameters.NetSerialize_Class(obj.UseData, writer);
        StaticNetSerializer_PersistentId.NetSerialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.NetSerialize(obj, writer);
    }

    public static SimPlayerInputUseItem NetDeserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInputUseItem obj = new SimPlayerInputUseItem();
        NetDeserialize(obj, reader);
        return obj;
    }
    public static void NetDeserialize(SimPlayerInputUseItem obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.ItemIndex, reader);
        obj.UseData = StaticNetSerializer_GameAction_UseParameters.NetDeserialize_Class(reader);
        StaticNetSerializer_PersistentId.NetDeserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.NetDeserialize(obj, reader);
    }
}

public static class ArrayNetSerializer_GameAction_ParameterData
{
    public static int GetNetBitSize(ref GameAction.ParameterData[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_GameAction_ParameterData.GetNetBitSize_Class(obj[i]);
        }
        return result;
    }

    public static void NetSerialize(ref GameAction.ParameterData[] obj, BitStreamWriter writer)
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
            StaticNetSerializer_GameAction_ParameterData.NetSerialize_Class(obj[i], writer);
        }
    }

    public static void NetDeserialize(ref GameAction.ParameterData[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new GameAction.ParameterData[reader.ReadInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i] = StaticNetSerializer_GameAction_ParameterData.NetDeserialize_Class(reader);
        }
    }
}
