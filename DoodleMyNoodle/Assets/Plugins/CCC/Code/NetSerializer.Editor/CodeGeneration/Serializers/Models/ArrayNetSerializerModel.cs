using System;
using System.IO;
using UnityEngine;
using UnityEngineX;

public static partial class NetSerializerCodeGenerator
{
    public static class ArrayNetSerializerModel
    {
        public static void Generate(Type arrayType, string completePath, bool clear, bool appendInFile)
        {
            Type elementType = arrayType.GetElementType();
            string elementFullName = elementType.GetPrettyFullName();

            StreamWriter writer = GetFileStream(completePath);

            bool success = true;

            if (!appendInFile)
            {
                writer.WriteLine("// THIS CODE IS GENERATED");
                writer.WriteLine("// DO NOT MODIFY IT");
                writer.WriteLine();
                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
            }

            writer.WriteLine();
            writer.WriteLine("public static class " + NetSerializationCodeGenUtility.GetSerializerNameFromType(arrayType));
            writer.WriteLine("{");
            writer.WriteLine("    public static int GetNetBitSize(ref " + elementFullName + "[] obj)");
            writer.WriteLine("    {");

            if (clear)
            {
                writer.WriteLine("        return 0;");
            }
            else
            {
                writer.WriteLine("        if (obj == null)");
                writer.WriteLine("            return 1;");
                writer.WriteLine("        int result = 1 + sizeof(Int32) * 8;");
                writer.WriteLine("        for (int i = 0; i < obj.Length; i++)");
                writer.WriteLine("        {");
                writer.WriteLine("            " + ModelHelpers.GetSerializerFieldLine_GetNetBitSize(elementType, "[i]"));
                writer.WriteLine("        }");
                writer.WriteLine("        return result;");
            }

            writer.WriteLine("    }");

            writer.WriteLine();

            writer.WriteLine("    public static void NetSerialize(ref " + elementFullName + "[] obj, BitStreamWriter writer)");
            writer.WriteLine("    {");
            if (!clear)
            {
                writer.WriteLine("        if (obj == null)");
                writer.WriteLine("        {");
                writer.WriteLine("            writer.WriteBit(false);");
                writer.WriteLine("            return;");
                writer.WriteLine("        }");
                writer.WriteLine("        writer.WriteBit(true);");
                writer.WriteLine("        writer.WriteInt32(obj.Length);");
                writer.WriteLine("        for (int i = 0; i < obj.Length; i++)");
                writer.WriteLine("        {");
                writer.WriteLine("            " + ModelHelpers.GetSerializerFieldLine_Serialize(elementType, "[i]"));
                writer.WriteLine("        }");
            }
            writer.WriteLine("    }");

            writer.WriteLine();

            writer.WriteLine("    public static void NetDeserialize(ref " + elementFullName + "[] obj, BitStreamReader reader)");
            writer.WriteLine("    {");
            if (!clear)
            {
                writer.WriteLine("        if (reader.ReadBit() == false)");
                writer.WriteLine("        {");
                writer.WriteLine("            obj = null;");
                writer.WriteLine("            return;");
                writer.WriteLine("        }");
                writer.WriteLine("        obj = new " + elementFullName + "[reader.ReadInt32()];");
                writer.WriteLine("        for (int i = 0; i < obj.Length; i++)");
                writer.WriteLine("        {");
                writer.WriteLine("            " + ModelHelpers.GetSerializerFieldLine_Deserialize(elementType, "[i]"));
                writer.WriteLine("        }");
            }
            writer.WriteLine("    }");

            writer.WriteLine("}");


            if (!success)
            {
                Debug.LogWarning("Error in code generation for type: " + elementType);
            }
        }
    }

    public static class ListNetSerializerModel
    {
        public static void Generate(Type listType, string completePath, bool clear, bool appendInFile)
        {
            Type elementType = listType.GetGenericArguments()[0];
            string elementFullName = elementType.GetPrettyFullName();

            StreamWriter writer = GetFileStream(completePath);

            bool success = true;

            if (!appendInFile)
            {
                writer.WriteLine("// THIS CODE IS GENERATED");
                writer.WriteLine("// DO NOT MODIFY IT");
                writer.WriteLine();
                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
            }

            writer.WriteLine();
            writer.WriteLine("public static class " + NetSerializationCodeGenUtility.GetSerializerNameFromType(listType));
            writer.WriteLine("{");
            writer.WriteLine($"    public static int GetNetBitSize_Class(List<{elementFullName}> obj)");
            writer.WriteLine("    {");

            if (clear)
            {
                writer.WriteLine("        return 0;");
            }
            else
            {
                writer.WriteLine("        if (obj == null)");
                writer.WriteLine("            return 1;");
                writer.WriteLine("        int result = 1 + sizeof(Int32) * 8;");
                writer.WriteLine("        for (int i = 0; i < obj.Count; i++)");
                writer.WriteLine("        {");
                writer.WriteLine("            var x = obj[i];");
                writer.WriteLine("            " + ModelHelpers.GetSerializerFieldLine_GetNetBitSize(elementType, null, "x"));
                writer.WriteLine("        }");
                writer.WriteLine("        return result;");
            }

            writer.WriteLine("    }");

            writer.WriteLine();

            writer.WriteLine($"    public static void NetSerialize_Class(List<{elementFullName}> obj, BitStreamWriter writer)");
            writer.WriteLine("    {");
            if (!clear)
            {
                writer.WriteLine("        if (obj == null)");
                writer.WriteLine("        {");
                writer.WriteLine("            writer.WriteBit(false);");
                writer.WriteLine("            return;");
                writer.WriteLine("        }");
                writer.WriteLine("        writer.WriteBit(true);");
                writer.WriteLine("        writer.WriteInt32(obj.Count);");
                writer.WriteLine("        for (int i = 0; i < obj.Count; i++)");
                writer.WriteLine("        {");
                writer.WriteLine("            var x = obj[i];");
                writer.WriteLine("            " + ModelHelpers.GetSerializerFieldLine_Serialize(elementType, null, "x"));
                writer.WriteLine("        }");
            }
            writer.WriteLine("    }");

            writer.WriteLine();

            writer.WriteLine($"    public static List<{elementFullName}> NetDeserialize_Class(BitStreamReader reader)");
            writer.WriteLine("    {");
            if (!clear)
            {
                writer.WriteLine("        if (reader.ReadBit() == false)");
                writer.WriteLine("        {");
                writer.WriteLine("            return null;");
                writer.WriteLine("        }");
                writer.WriteLine("        int size = reader.ReadInt32();");
                writer.WriteLine($"        List<{elementFullName}> obj = new List<{elementFullName}>(size);");
                writer.WriteLine("        for (int i = 0; i < size; i++)");
                writer.WriteLine("        {");
                writer.WriteLine($"            {elementFullName} x = default;");
                writer.WriteLine("            " + ModelHelpers.GetSerializerFieldLine_Deserialize(elementType, null, "x"));
                writer.WriteLine($"            obj.Add(x);");
                writer.WriteLine("        }");
                writer.WriteLine("        return obj;");
            }
            writer.WriteLine("    }");

            writer.WriteLine("}");


            if (!success)
            {
                Debug.LogWarning("Error in code generation for type: " + elementType);
            }
        }
    }
}
