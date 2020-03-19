using System;
using System.Reflection;
using System.Runtime.CompilerServices;

public static partial class NetSerializerCodeGenerator
{
    public static class ModelHelpers
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSerializerFieldLine_GetNetBitSizeBaseClass(Type baseType)
        {
            return $"result += {NetSerializationCodeGenUtility.GetSerializerNameFromType(baseType)}.GetNetBitSize(obj);";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSerializerFieldLine_GetNetBitSize(Type fieldType, string fieldAccessor)
        {
            if (fieldType.IsEnum)
            {
                return $"result += {NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.GetNetBitSize();";
            }
            if (NetSerializationCodeGenUtility.ConsideredAsValueType(fieldType))
            {
                return $"result += {NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.GetNetBitSize(ref obj{fieldAccessor});";
            }
            else
            {
                return $"result += {NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.GetNetBitSize_Class(obj{fieldAccessor});";
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSerializerFieldLine_SerializeBaseClass(Type baseType)
        {
            return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(baseType)}.NetSerialize(obj, writer);";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSerializerFieldLine_Serialize(Type fieldType, string fieldAccessor)
        {
            if (fieldType.IsEnum)
            {
                return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.NetSerialize(({fieldType.GetEnumUnderlyingType()})obj{fieldAccessor}, writer);";
            }
            if (NetSerializationCodeGenUtility.ConsideredAsValueType(fieldType))
            {
                return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.NetSerialize(ref obj{fieldAccessor}, writer);";
            }
            else
            {
                return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.NetSerialize_Class(obj{fieldAccessor}, writer);";
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSerializerFieldLine_DeserializeBaseClass(Type baseType)
        {
            return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(baseType)}.NetDeserialize(obj, reader);";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSerializerFieldLine_Deserialize(Type fieldType, string fieldAccessor)
        {
            if (fieldType.IsEnum)
            {
                return $"obj{fieldAccessor} = ({fieldType.GetNiceFullName()}){NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.NetDeserialize(reader);";
            }
            else if (NetSerializationCodeGenUtility.ConsideredAsValueType(fieldType))
            {
                return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.NetDeserialize(ref obj{fieldAccessor}, reader);";
            }
            else
            {
                return $"obj{fieldAccessor} = {NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.NetDeserialize_Class(reader);";
            }
        }
    }
}