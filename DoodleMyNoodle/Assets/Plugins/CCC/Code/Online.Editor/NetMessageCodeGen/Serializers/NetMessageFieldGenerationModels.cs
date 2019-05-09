//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Reflection;
//using UnityEngine;

//public static class NetMessageFieldGenerationModels
//{
//    public struct FieldData
//    {
//        public FieldData(FieldInfo fieldInfo)
//        {
//            type = fieldInfo.FieldType;
//            name = fieldInfo.Name;
//        }
//        public FieldData(Type fieldType, string overrideName)
//        {
//            type = fieldType;
//            name = overrideName;
//        }

//        public readonly Type type;
//        public readonly string name;
//    }

//    public class GenerationModel
//    {
//        public Func<FieldData, bool> evaluate;
//        public Action<FieldData, Action<string>> netByteSizeAction;
//        public Action<FieldData, Action<string>> networkSerializeAction;
//        public Action<FieldData, Action<string>> networkDeserializeAction;
//    }

//    public static GenerationModel GetCodeGenerationModel(FieldData fieldInfo)
//    {
//        for (int i = 0; i < serializedTypes.Length; i++)
//        {
//            if (serializedTypes[i].evaluate(fieldInfo))
//            {
//                return serializedTypes[i];
//            }
//        }
//        return null;
//    }

//    public static bool WriteNetByteSizeLines(FieldData field, Action<string> write)
//    {
//        try
//        {
//            var codeGenModel = GetCodeGenerationModel(field);
//            if (codeGenModel == null)
//            {
//                Debug.LogWarning("No CodeGeneration model for field type: " + field.type);
//                return false;
//            }
//            else
//            {
//                codeGenModel.netByteSizeAction(field, write);
//            }
//            return true;
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Error in generation with field type: " + field.type + "    error:" + e.Message);
//            return false;
//        }
//    }
//    public static bool WriteNetSerializeLines(FieldData field, Action<string> write)
//    {
//        try
//        {
//            var codeGenModel = GetCodeGenerationModel(field);
//            if (codeGenModel != null)
//            {
//                codeGenModel.networkSerializeAction(field, write);
//            }
//            return true;
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Error in generation with field type: " + field.type + "    error:" + e.Message);
//            return false;
//        }
//    }
//    public static bool WriteNetDeserializeLines(FieldData field, Action<string> write)
//    {
//        try
//        {
//            var codeGenModel = GetCodeGenerationModel(field);
//            if (codeGenModel != null)
//            {
//                codeGenModel.networkDeserializeAction(field, write);
//            }
//            return true;
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("Error in generation with field type: " + field.type + "    error:" + e.Message);
//            return false;
//        }
//    }


//    public static GenerationModel[] serializedTypes =
//    {
//        new GenerationModel()
//        {
//            evaluate = (field) => field.type == typeof(Int16)
//            ,
//            netByteSizeAction = (field, write) => write("result += " + sizeof(Int16)*8 + ';')
//            ,
//            networkSerializeAction = (field, write) => write("writer.WriteInt16(" + field.name + ");")
//            ,
//            networkDeserializeAction = (field, write) => write(field.name +" = reader.ReadInt16();")
//        }
//        ,
//        new GenerationModel()
//        {
//            evaluate = (field) => field.type == typeof(UInt16)
//            ,
//            netByteSizeAction = (field, write) => write("result += " + sizeof(UInt16)*8 + ';')
//            ,
//            networkSerializeAction = (field, write) => write("writer.WriteUInt16(" + field.name + ");")
//            ,
//            networkDeserializeAction = (field, write) => write(field.name +" = reader.ReadUInt16();")
//        }
//        ,
//        new GenerationModel()
//        {
//            evaluate = (field) => field.type == typeof(Int32)
//            ,
//            netByteSizeAction = (field, write) => write("result += " + sizeof(Int32)*8 + ';')
//            ,
//            networkSerializeAction = (field, write) => write("writer.WriteInt32(" + field.name + ");")
//            ,
//            networkDeserializeAction = (field, write) => write(field.name +" = reader.ReadInt32();")
//        }
//        ,
//        new GenerationModel()
//        {
//            evaluate = (field) => field.type == typeof(UInt32)
//            ,
//            netByteSizeAction = (field, write) => write("result += " + sizeof(UInt32)*8 + ';')
//            ,
//            networkSerializeAction = (field, write) => write("writer.WriteUInt32(" + field.name + ");")
//            ,
//            networkDeserializeAction = (field, write) => write(field.name +" = reader.ReadUInt32();")
//        }
//        ,
//        new GenerationModel()
//        {
//            evaluate = (field) => field.type == typeof(byte)
//            ,
//            netByteSizeAction = (field, write) => write("result += " + sizeof(byte)*8 + ';')
//            ,
//            networkSerializeAction = (field, write) => write("writer.WriteByte(" + field.name + ");")
//            ,
//            networkDeserializeAction = (field, write) => write(field.name +" = reader.ReadByte();")
//        }
//        ,
//        new GenerationModel()
//        {
//            evaluate = (field) => field.type == typeof(Boolean)
//            ,
//            netByteSizeAction = (field, write) => write("result += 1;")
//            ,
//            networkSerializeAction = (field, write) => write("writer.WriteBool(" + field.name + ");")
//            ,
//            networkDeserializeAction = (field, write) => write(field.name +" = reader.ReadBool();")
//        }
//        ,
//        new GenerationModel()
//        {
//            evaluate = (field) => field.type == typeof(String)
//            ,
//            netByteSizeAction = (field, write) => write("result += " + field.name + ".Length * " + sizeof(char)*8 + " + " + sizeof(UInt16) * 8 + ";")
//            ,
//            networkSerializeAction = (field, write) => write("writer.WriteString(" + field.name + ");")
//            ,
//            networkDeserializeAction = (field, write) => write(field.name +" = reader.ReadString();")
//        }
//        //,
//        //new GenerationModel()
//        //{
//        //    evaluate = (field) => typeof(INetSerializable).IsAssignableFrom(field.type) && field.type.IsClass   // classes
//        //    ,
//        //    netByteSizeAction = (field, write) => write("result += " + field.name + " == null ? 0 : " + field.name +".GetNetBitSize();")
//        //    ,
//        //    networkSerializeAction = (field, write) => write(field.name + ".NetSerialize(writer);")
//        //    ,
//        //    networkDeserializeAction = (field, write) => write(field.name + ".NetDeserialize(reader);")
//        //}
//        ,
//        new GenerationModel()
//        {
//            evaluate = (field) => typeof(INetSerializable).IsAssignableFrom(field.type) && !field.type.IsClass   //structs
//            ,
//            netByteSizeAction = (field, write) => write("result += " + field.name + ".GetNetBitSize();")
//            ,
//            networkSerializeAction = (field, write) => write(field.name + ".NetSerialize(writer);")
//            ,
//            networkDeserializeAction = (field, write) => write(field.name + ".NetDeserialize(reader);")
//        }
//        ,
//        new GenerationModel()
//        {
//            evaluate = (field) =>
//            {
//                return typeof(IList).IsAssignableFrom(field.type)
//                    && typeof(ICollection).IsAssignableFrom(field.type)
//                    && field.type.IsGenericType
//                    && typeof(INetSerializable).IsAssignableFrom(field.type.GenericTypeArguments[0]);
//            }
//            ,
//            netByteSizeAction = (field, write) => write("result += " + field.name + ".GetNetBitSize();")
//            ,
//            networkSerializeAction = (field, write) => write(field.name + ".NetSerialize(writer);")
//            ,
//            networkDeserializeAction = (field, write) => write(field.name + ".NetDeserialize(reader);")
//        }
//        ,
//        new GenerationModel()
//        {
//            evaluate = (field) =>
//            {
//                return typeof(IList).IsAssignableFrom(field.type)
//                    && typeof(ICollection).IsAssignableFrom(field.type)
//                    && field.type.IsGenericType;
//            }
//            ,
//            netByteSizeAction = (field, write) =>
//            {
//                Type genericType = field.type.GenericTypeArguments[0];
//                string iVariableName = field.name + "_i";

//                write("result += " + sizeof(ushort) * 8 + "; // list size");
//                write("for (int " + iVariableName + " = 0; " + iVariableName + " < " + field.name + ".Count; " + iVariableName + "++)");
//                write("{");
//                Action<string> lineAction = (s)=> write("    " + s);
//                WriteNetByteSizeLines(new FieldData(genericType, field.name + "[" + iVariableName + "]"), lineAction);
//                write("}");
//            }
//            ,
//            networkSerializeAction = (field, write) =>
//            {
//                Type genericType = field.type.GenericTypeArguments[0];
//                string iVariableName = field.name + "_i";

//                write("writer.WriteUInt16((UInt16)" + field.name + ".Count);");
//                write("for (ushort " + iVariableName + " = 0; " + iVariableName + " < " + field.name + ".Count; " + iVariableName + "++)");
//                write("{");
//                Action<string> lineAction = (s)=> write("    " + s);
//                WriteNetSerializeLines(new FieldData(genericType, field.name + "[" + iVariableName + "]"), lineAction);
//                write("}");
//            }
//            ,
//            networkDeserializeAction = (field, write) =>
//            {
//                Type genericType = field.type.GenericTypeArguments[0];

//                string countVariableName = field.name + "_count";
//                string iVariableName = field.name + "_i";
//                write("int " + countVariableName + " = reader.ReadUInt16();");

//                write("if (" + field.name + ".Capacity < " + countVariableName + ")");
//                write("    " + field.name + ".Capacity = " + countVariableName + " * 2;");
//                write("for (ushort " + iVariableName + " = 0; " + iVariableName + " < " + countVariableName + "; " + iVariableName + "++)");
//                write("{");
//                write("    if (" + iVariableName + " == " + field.name + ".Count)");
//                write("        " + field.name + ".Add(new " + genericType + "());");
//                Action<string> lineAction = (s)=> write("    " + s);
//                WriteNetDeserializeLines(new FieldData(genericType, field.name + "[" + iVariableName + "]"), lineAction);
//                write("}");

//                write("if(" + field.name + ".Count > " + countVariableName + ")");
//                write("{");
//                write("    " + field.name + ".RemoveRange(" + countVariableName + ", " + field.name + ".Count - " + countVariableName + ");");
//                write("}");

//            }
//        }
//    };

//}
