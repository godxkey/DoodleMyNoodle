using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public static partial class NetSerializerCodeGenerator
{
    public static class StaticNetSerializerModel
    {
        public static void Generate(Type type, string filePath, string fileName, bool clear)
        {
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string completePath = filePath + '/' + fileName;

            if (!File.Exists(completePath))
            {
                File.Create(completePath).Close();
            }


            NetSerializableAttribute serializableAttribute = type.GetCustomAttribute<NetSerializableAttribute>();

            List<FieldInfo> fields = new List<FieldInfo>(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
            fields.RemoveAll((f) => NetSerializationCodeGenUtility.ShouldIgnoreCodeGeneration(f));


            Type baseClass = type.BaseType == typeof(object) || type.BaseType == typeof(ValueType) ? null : type.BaseType;
            bool couldBeDynamic = !NetSerializationCodeGenUtility.ConsideredAsValueType(type);
            bool isDynamicType = serializableAttribute != null ? serializableAttribute.isBaseClass : false;
            string methodObjParameter_Rcv = couldBeDynamic ?
                (type.Name + " obj") :
                ("ref " + type.Name + " obj");


            using (FileStream fileStream = File.Open(completePath, FileMode.Truncate))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {

                    writer.Flush();

                    writer.Write(
@"// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
");


                    writer.WriteLine("public static class " + NetSerializationCodeGenUtility.GetSerializerNameFromType(type));
                    writer.WriteLine("{");

                    if (couldBeDynamic)
                    {
                        writer.WriteLine("    public static int GetNetBitSize_Class(" + methodObjParameter_Rcv + ")");
                        writer.WriteLine("    {");
                        if (clear)
                        {
                            writer.WriteLine("        return 0;");
                        }
                        else
                        {
                            writer.WriteLine("        if (obj == null)");
                            writer.WriteLine("            return 1;");
                            if (isDynamicType)
                            {
                                writer.WriteLine("        return 1 + DynamicNetSerializer.GetNetBitSize(obj);");
                            }
                            else
                            {
                                writer.WriteLine("        return 1 + GetNetBitSize(obj);");
                            }
                        }
                        writer.WriteLine("    }");
                        writer.WriteLine();
                    }

                    writer.WriteLine("    public static int GetNetBitSize(" + methodObjParameter_Rcv + ")");
                    writer.WriteLine("    {");

                    if (clear)
                    {
                        writer.WriteLine("        return 0;");
                    }
                    else
                    {
                        writer.WriteLine("        int result = 0;");

                        foreach (var field in fields)
                        {
                            if (field.FieldType.IsArray && !generateArrayCode.Contains(field.FieldType))
                            {
                                generateArrayCode.Add(field.FieldType);
                            }
                            writer.WriteLine("        " + ModelHelpers.GetSerializerFieldLine_GetNetBitSize(field.FieldType, '.' + field.Name));
                        }
                        if (baseClass != null)
                            writer.WriteLine("        " + ModelHelpers.GetSerializerFieldLine_GetNetBitSizeBaseClass(baseClass));
                        writer.WriteLine("        return result;");
                    }

                    writer.WriteLine("    }");

                    writer.WriteLine();


                    if (couldBeDynamic)
                    {

                        writer.WriteLine("    public static void NetSerialize_Class(" + methodObjParameter_Rcv + ", BitStreamWriter writer)");
                        writer.WriteLine("    {");
                        if (clear)
                        {
                            // nothing
                        }
                        else
                        {
                            writer.WriteLine("        if (obj == null)");
                            writer.WriteLine("        {");
                            writer.WriteLine("            writer.WriteBit(false);");
                            writer.WriteLine("            return;");
                            writer.WriteLine("        }");
                            writer.WriteLine("        writer.WriteBit(true);");

                            if (isDynamicType)
                            {
                                writer.WriteLine("        DynamicNetSerializer.NetSerialize(obj, writer);");
                            }
                            else
                            {
                                writer.WriteLine("        NetSerialize(obj, writer);");
                            }
                        }
                        writer.WriteLine("    }");
                    }


                    writer.WriteLine("    public static void NetSerialize(" + methodObjParameter_Rcv + ", BitStreamWriter writer)");
                    writer.WriteLine("    {");
                    if (!clear)
                    {
                        foreach (var field in fields)
                        {
                            writer.WriteLine("        " + ModelHelpers.GetSerializerFieldLine_Serialize(field.FieldType, '.' + field.Name));
                        }
                        if (baseClass != null)
                            writer.WriteLine("        " + ModelHelpers.GetSerializerFieldLine_SerializeBaseClass(baseClass));
                    }
                    writer.WriteLine("    }");

                    writer.WriteLine();

                    if (couldBeDynamic)
                    {
                        writer.WriteLine("    public static " + type.Name + " NetDeserialize_Class(BitStreamReader reader)");
                        writer.WriteLine("    {");
                        if (clear)
                        {
                            writer.WriteLine("        return null;");
                        }
                        else
                        {
                            writer.WriteLine("        if (reader.ReadBit() == false)");
                            writer.WriteLine("        {");
                            writer.WriteLine("            return null;");
                            writer.WriteLine("        }");

                            if (isDynamicType)
                            {
                                writer.WriteLine("        return (" + type.Name + ")DynamicNetSerializer.NetDeserialize(reader);");
                            }
                            else
                            {
                                writer.WriteLine("        " + type.Name + " obj = new " + type.Name + "();");
                                writer.WriteLine("        NetDeserialize(obj, reader);");
                                writer.WriteLine("        return obj;");
                            }
                        }
                        writer.WriteLine("    }");
                    }


                    writer.WriteLine("    public static void NetDeserialize(" + methodObjParameter_Rcv + ", BitStreamReader reader)");
                    writer.WriteLine("    {");
                    if (!clear)
                    {
                        foreach (var field in fields)
                        {
                            writer.WriteLine("        " + ModelHelpers.GetSerializerFieldLine_Deserialize(field.FieldType, '.' + field.Name));
                        }
                        if (baseClass != null)
                            writer.WriteLine("        " + ModelHelpers.GetSerializerFieldLine_DeserializeBaseClass(baseClass));
                    }
                    writer.WriteLine("    }");

                    writer.WriteLine("}");
                }
            }
        }
    }
}