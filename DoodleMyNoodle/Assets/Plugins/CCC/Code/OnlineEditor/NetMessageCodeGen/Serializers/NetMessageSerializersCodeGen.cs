using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public static partial class NetMessageSerializersCodeGenerator
{
    static Dictionary<string, string> assemblyToFilePath = new Dictionary<string, string>()
    {
        {"Model", "Assets/Code/Model/Generated/NetMessageSerializers"}
        , {"Game", "Assets/Code/Game/Generated/NetMessageSerializers"}
        , {"mscorlib", "Assets/Plugins/CCC/Code/Online/Generated/NetMessageSerializers"}
        //, {"Examples", "Assets/Code/Examples/Generated/NetMessageSerializers"}
    };

    static HashSet<Type> doNoRegenerate = new HashSet<Type>();
    static HashSet<Type> generateArrayCode = new HashSet<Type>();


    [MenuItem("Tools/Code Generation/NetMessage Serializers/Generate")]
    static void Generate()
    {
        Generate(false);
    }

    [MenuItem("Tools/Code Generation/NetMessage Serializers/Clear")]
    static void Clear()
    {
        Generate(true);
    }

    static void Generate(bool clear)
    {
        doNoRegenerate = new HashSet<Type>(pregeneratedSerializers);
        generateArrayCode.Clear();

        ReadOnlyCollection<Type> netMessageTypes = NetMessageCodeGenUtility.GetNetMessageTypes();

        foreach (Type type in netMessageTypes)
        {
            GenerateCode(type, clear);
        }

        foreach (Type type in generateArrayCode)
        {
            GenerateCode(type, clear);
        }

        AssetDatabase.Refresh();
    }

    static void GenerateCode(Type type, bool clear)
    {
        if (NetMessageAttributes.ShouldIgnoreCodeGeneration(type) || doNoRegenerate.Contains(type))
        {
            return;
        }

        string assemblyName = type.Assembly.GetName().Name;
        if (assemblyToFilePath.ContainsKey(assemblyName))
        {
            doNoRegenerate.Add(type);

            if (type.IsArray)
            {
                GenerateArrayCode(
                    type,
                    assemblyToFilePath[assemblyName],
                    NetMessageCodeGenUtility.GetSerializerNameFromType(type) + ".generated.cs",
                    clear);
            }
            else
            {
                GenerateCode(
                    type,
                    assemblyToFilePath[assemblyName],
                    NetMessageCodeGenUtility.GetSerializerNameFromType(type) + ".generated.cs",
                    clear);
            }

        }
        else
        {
            Debug.LogError("No assembly->filePath association found in dictionary for " + assemblyName);
        }
    }

    static void GenerateCode(Type type, string filePath, string fileName, bool clear)
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


        List<FieldInfo> fields = new List<FieldInfo>(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

        fields.RemoveAll((f) => NetMessageAttributes.ShouldIgnoreCodeGeneration(f));

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
                writer.WriteLine("public static class " + NetMessageCodeGenUtility.GetSerializerNameFromType(type));
                writer.WriteLine("{");
                writer.WriteLine("    public static int GetNetBitSize(ref " + type.Name + " obj)");
                writer.WriteLine("    {");

                if (clear)
                {
                    writer.WriteLine("        return 0;");
                }
                else
                {
                    if (type.IsClass)
                    {
                        writer.WriteLine("        if (obj == null)");
                        writer.WriteLine("            return 1;");
                        writer.WriteLine("        int result = 1;");
                    }
                    else
                    {
                        writer.WriteLine("        int result = 0;");
                    }

                    foreach (var field in fields)
                    {
                        if (field.FieldType.IsArray && !generateArrayCode.Contains(field.FieldType))
                        {
                            generateArrayCode.Add(field.FieldType);
                        }
                        writer.WriteLine("        result += " + NetMessageCodeGenUtility.GetSerializerNameFromType(field.FieldType) + ".GetNetBitSize(ref obj." + field.Name + ");");
                    }
                    writer.WriteLine("        return result;");
                }

                writer.WriteLine("    }");

                writer.WriteLine();

                writer.WriteLine("    public static void NetSerialize(ref " + type.Name + " obj, BitStreamWriter writer)");
                writer.WriteLine("    {");
                if (!clear)
                {
                    if (type.IsClass)
                    {
                        writer.WriteLine("        if (obj == null)");
                        writer.WriteLine("        {");
                        writer.WriteLine("            writer.WriteBit(false);");
                        writer.WriteLine("            return;");
                        writer.WriteLine("        }");
                        writer.WriteLine("        writer.WriteBit(true);");
                    }
                    else
                    {
                        // nothing
                    }
                    foreach (var field in fields)
                    {
                        writer.WriteLine("        " + NetMessageCodeGenUtility.GetSerializerNameFromType(field.FieldType) + ".NetSerialize(ref obj." + field.Name + ", writer);");
                    }
                }
                writer.WriteLine("    }");

                writer.WriteLine();

                writer.WriteLine("    public static void NetDeserialize(ref " + type.Name + " obj, BitStreamReader reader)");
                writer.WriteLine("    {");
                if (!clear)
                {
                    if (type.IsClass)
                    {
                        writer.WriteLine("        if (reader.ReadBit() == false)");
                        writer.WriteLine("        {");
                        writer.WriteLine("            obj = null;");
                        writer.WriteLine("            return;");
                        writer.WriteLine("        }");
                        writer.WriteLine("        obj = new " + type.Name + "();");
                    }
                    else
                    {
                        // nothing
                    }
                    foreach (var field in fields)
                    {
                        writer.WriteLine("        " + NetMessageCodeGenUtility.GetSerializerNameFromType(field.FieldType) + ".NetDeserialize(ref obj." + field.Name + ", reader);");
                    }
                }
                writer.WriteLine("    }");

                writer.WriteLine("}");


                if (!success)
                {
                    Debug.LogWarning("Error in code generation for type: " + type);
                }
            }
        }
    }

    static void GenerateArrayCode(Type arrayType, string filePath, string fileName, bool clear)
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
                writer.WriteLine("public static class " + NetMessageCodeGenUtility.GetSerializerNameFromType(arrayType));
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
                    writer.WriteLine("            result += " + NetMessageCodeGenUtility.GetSerializerNameFromType(elementType) + ".GetNetBitSize(ref obj[i]);");
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
                    writer.WriteLine("            " + NetMessageCodeGenUtility.GetSerializerNameFromType(elementType) + ".NetSerialize(ref obj[i], writer);");
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
                    writer.WriteLine("            " + NetMessageCodeGenUtility.GetSerializerNameFromType(elementType) + ".NetDeserialize(ref obj[i], reader);");
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