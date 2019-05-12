using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static partial class NetSerializerCodeGenerator
{
    public static class ArrayNetSerializerModel
    {
        public static void Generate(Type arrayType, string filePath, string fileName, bool clear)
        {
            Type elementType = arrayType.GetElementType();

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string completePath = filePath + '/' + fileName;

            if (!File.Exists(completePath))
            {
                File.Create(completePath).Close();
            }

            using (FileStream fileStream = File.Open(completePath, FileMode.Truncate))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    bool success = true;
                    writer.Flush();

                    writer.WriteLine("// THIS CODE IS GENERATED");
                    writer.WriteLine("// DO NOT MODIFY IT");
                    writer.WriteLine();
                    writer.WriteLine("using System;");
                    writer.WriteLine("using System.Collections.Generic;");
                    writer.WriteLine();
                    writer.WriteLine("public static class " + NetSerializationCodeGenUtility.GetSerializerNameFromType(arrayType));
                    writer.WriteLine("{");
                    writer.WriteLine("    public static int GetNetBitSize(ref " + elementType.Name + "[] obj)");
                    writer.WriteLine("    {");

                    if (clear)
                    {
                        writer.WriteLine("        return 0;");
                    }
                    else
                    {
                        writer.WriteLine("        if (obj == null)");
                        writer.WriteLine("            return 1;");
                        writer.WriteLine("        int result = 1 + sizeof(UInt16) * 8;");
                        writer.WriteLine("        for (int i = 0; i < obj.Length; i++)");
                        writer.WriteLine("        {");
                        writer.WriteLine("            " + ModelHelpers.GetSerializerFieldLine_GetNetBitSize(elementType, "[i]"));
                        writer.WriteLine("        }");
                        writer.WriteLine("        return result;");
                    }

                    writer.WriteLine("    }");

                    writer.WriteLine();

                    writer.WriteLine("    public static void NetSerialize(ref " + elementType.Name + "[] obj, BitStreamWriter writer)");
                    writer.WriteLine("    {");
                    if (!clear)
                    {
                        writer.WriteLine("        if (obj == null)");
                        writer.WriteLine("        {");
                        writer.WriteLine("            writer.WriteBit(false);");
                        writer.WriteLine("            return;");
                        writer.WriteLine("        }");
                        writer.WriteLine("        writer.WriteBit(true);");
                        writer.WriteLine("        writer.WriteUInt16((UInt16)obj.Length);");
                        writer.WriteLine("        for (int i = 0; i < obj.Length; i++)");
                        writer.WriteLine("        {");
                        writer.WriteLine("            " + ModelHelpers.GetSerializerFieldLine_Serialize(elementType, "[i]"));
                        writer.WriteLine("        }");
                    }
                    writer.WriteLine("    }");

                    writer.WriteLine();

                    writer.WriteLine("    public static void NetDeserialize(ref " + elementType.Name + "[] obj, BitStreamReader reader)");
                    writer.WriteLine("    {");
                    if (!clear)
                    {
                        writer.WriteLine("        if (reader.ReadBit() == false)");
                        writer.WriteLine("        {");
                        writer.WriteLine("            obj = null;");
                        writer.WriteLine("            return;");
                        writer.WriteLine("        }");
                        writer.WriteLine("        obj = new " + elementType.Name + "[reader.ReadUInt16()];");
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
        }
    }
}