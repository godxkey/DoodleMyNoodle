using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngineX;

public static partial class NetSerializerCodeGenerator
{
    public static class ModelHelpers
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSerializerFieldLine_GetNetBitSizeBaseClass(Type baseType)
        {
            return $"result += {NetSerializationCodeGenUtility.GetSerializerNameFromType(baseType)}.GetNetBitSize(obj);";
        }

        public static string GetSerializerFieldLine_GetNetBitSize(Type fieldType, string fieldAccessor, string fullFieldAccessor = null)
        {
            if (fullFieldAccessor == null)
            {
                fullFieldAccessor = "obj" + fieldAccessor;
            }

            if (fieldType.IsEnum)
            {
                return $"result += {NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.GetNetBitSize();";
            }
            if (NetSerializationCodeGenUtility.ConsideredAsValueType(fieldType))
            {
                return $"result += {NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.GetNetBitSize(ref {fullFieldAccessor});";
            }
            else
            {
                return $"result += {NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.GetNetBitSize_Class({fullFieldAccessor});";
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSerializerFieldLine_SerializeBaseClass(Type baseType)
        {
            return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(baseType)}.NetSerialize(obj, writer);";
        }

        public static string GetSerializerFieldLine_Serialize(Type fieldType, string fieldAccessor, string fullFieldAccessor = null)
        {
            if (fullFieldAccessor == null)
            {
                fullFieldAccessor = "obj" + fieldAccessor;
            }

            if (fieldType.IsEnum)
            {
                return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.NetSerialize(({fieldType.GetEnumUnderlyingType()}){fullFieldAccessor}, writer);";
            }
            if (NetSerializationCodeGenUtility.ConsideredAsValueType(fieldType))
            {
                return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.NetSerialize(ref {fullFieldAccessor}, writer);";
            }
            else
            {
                return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.NetSerialize_Class({fullFieldAccessor}, writer);";
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSerializerFieldLine_DeserializeBaseClass(Type baseType)
        {
            return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(baseType)}.NetDeserialize(obj, reader);";
        }

        public static string GetSerializerFieldLine_Deserialize(Type fieldType, string fieldAccessor, string fullFieldAccessor = null)
        {
            if(fullFieldAccessor == null)
            {
                fullFieldAccessor = "obj" + fieldAccessor;
            }

            if (fieldType.IsEnum)
            {
                return $"{fullFieldAccessor} = ({fieldType.GetPrettyFullName()}){NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.NetDeserialize(reader);";
            }
            else if (NetSerializationCodeGenUtility.ConsideredAsValueType(fieldType))
            {
                return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.NetDeserialize(ref {fullFieldAccessor}, reader);";
            }
            else
            {
                return $"{fullFieldAccessor} = {NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.NetDeserialize_Class(reader);";
            }
        }
    }
}