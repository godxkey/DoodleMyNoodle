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
            return $"result += {NetSerializationCodeGenUtility.GetSerializerNameFromType(baseType)}.GetSerializedBitSize(obj);";
        }

        public static string GetSerializerFieldLine_GetNetBitSize(Type fieldType, string fieldAccessor, string fullFieldAccessor = null)
        {
            if (fullFieldAccessor == null)
            {
                fullFieldAccessor = "obj" + fieldAccessor;
            }

            if (fieldType.IsEnum)
            {
                return $"result += {NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.GetSerializedBitSize();";
            }
            if (NetSerializationCodeGenUtility.ConsideredAsValueType(fieldType))
            {
                return $"result += {NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.GetSerializedBitSize(ref {fullFieldAccessor});";
            }
            else
            {
                return $"result += {NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.GetSerializedBitSize_Class({fullFieldAccessor});";
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSerializerFieldLine_SerializeBaseClass(Type baseType)
        {
            return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(baseType)}.Serialize(obj, writer);";
        }

        public static string GetSerializerFieldLine_Serialize(Type fieldType, string fieldAccessor, string fullFieldAccessor = null)
        {
            if (fullFieldAccessor == null)
            {
                fullFieldAccessor = "obj" + fieldAccessor;
            }

            if (fieldType.IsEnum)
            {
                return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.Serialize(({fieldType.GetEnumUnderlyingType()}){fullFieldAccessor}, writer);";
            }
            if (NetSerializationCodeGenUtility.ConsideredAsValueType(fieldType))
            {
                return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.Serialize(ref {fullFieldAccessor}, writer);";
            }
            else
            {
                return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.Serialize_Class({fullFieldAccessor}, writer);";
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSerializerFieldLine_DeserializeBaseClass(Type baseType)
        {
            return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(baseType)}.Deserialize(obj, reader);";
        }

        public static string GetSerializerFieldLine_Deserialize(Type fieldType, string fieldAccessor, string fullFieldAccessor = null)
        {
            if(fullFieldAccessor == null)
            {
                fullFieldAccessor = "obj" + fieldAccessor;
            }

            if (fieldType.IsEnum)
            {
                return $"{fullFieldAccessor} = ({fieldType.GetPrettyFullName()}){NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.Deserialize(reader);";
            }
            else if (NetSerializationCodeGenUtility.ConsideredAsValueType(fieldType))
            {
                return $"{NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.Deserialize(ref {fullFieldAccessor}, reader);";
            }
            else
            {
                return $"{fullFieldAccessor} = {NetSerializationCodeGenUtility.GetSerializerNameFromType(fieldType)}.Deserialize_Class(reader);";
            }
        }
    }
}