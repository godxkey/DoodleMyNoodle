using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class NetMessageSerializersCodeGenerator
{
    static Dictionary<string, string> assemblyToFilePath = new Dictionary<string, string>()
    {
        {"Model", "Assets/Code/Model/Generated/NetMessageSerializers"}
        , {"Game", "Assets/Code/Game/Generated/NetMessageSerializers"}
        //, {"Examples", "Assets/Code/Examples/Generated/NetMessageSerializers"}
    };




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
        ReadOnlyCollection<Type> netMessageTypes = NetMessageCodeGenerationUtility.GetNetMessageTypes();

        foreach (Type type in netMessageTypes)
        {
            if (NetMessageAttributes.ShouldIgnoreCodeGeneration(type))
            {
                continue;
            }

            string assemblyName = type.Assembly.GetName().Name;
            if (assemblyToFilePath.ContainsKey(assemblyName))
            {
                GenerateCode(type, assemblyToFilePath[assemblyName], type.Name + ".generated.cs", clear);
            }
            else
            {
                Debug.LogError("No assembly->filePath association found in dictionary for " + assemblyName);
            }
        }

        AssetDatabase.Refresh();
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
                bool isOverride = type.IsSubclassOf(typeof(NetMessage));

                writer.Flush();

                writer.WriteLine("// THIS CODE IS GENERATED");
                writer.WriteLine("// DO NOT MODIFY IT");
                writer.WriteLine();
                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine();
                if (type.IsClass)
                {
                    writer.WriteLine("public partial class " + type.Name);
                }
                else
                {
                    writer.WriteLine("public partial struct " + type.Name);
                }
                writer.WriteLine("{");

                Action<string> lineAction;

                if (isOverride)
                    writer.WriteLine("    public override int GetNetBitSize()");
                else
                    writer.WriteLine("    public int GetNetBitSize()");
                writer.WriteLine("    {");
                writer.WriteLine("        int result = 0;");
                lineAction = (s) => writer.WriteLine("        " + s);
                if (!clear)
                {
                    if (isOverride)
                        writer.WriteLine("        result += base.GetNetBitSize();");
                    foreach (FieldInfo fieldInfo in fields)
                    {
                        success &= NetMessageFieldGenerationModels.WriteNetByteSizeLines(new NetMessageFieldGenerationModels.FieldData(fieldInfo), lineAction);
                    }
                }
                writer.WriteLine("        return result;");
                writer.WriteLine("    }");

                writer.WriteLine();


                if (isOverride)
                    writer.WriteLine("    public override void NetSerialize(BitStreamWriter writer)");
                else
                    writer.WriteLine("    public void NetSerialize(BitStreamWriter writer)");
                writer.WriteLine("    {");
                if (!clear)
                {
                    if (isOverride)
                        writer.WriteLine("        base.NetSerialize(writer);");
                    lineAction = (s) => writer.WriteLine("        " + s);
                    foreach (FieldInfo fieldInfo in fields)
                    {
                        success &= NetMessageFieldGenerationModels.WriteNetSerializeLines(new NetMessageFieldGenerationModels.FieldData(fieldInfo), lineAction);
                    }
                }
                writer.WriteLine("    }");


                writer.WriteLine();


                if (isOverride)
                    writer.WriteLine("    public override void NetDeserialize(BitStreamReader reader)");
                else
                    writer.WriteLine("    public void NetDeserialize(BitStreamReader reader)");
                writer.WriteLine("    {");
                if (!clear)
                {
                    if (isOverride)
                        writer.WriteLine("        base.NetDeserialize(reader);");
                    lineAction = (s) => writer.WriteLine("        " + s);
                    foreach (FieldInfo fieldInfo in fields)
                    {
                        success &= NetMessageFieldGenerationModels.WriteNetDeserializeLines(new NetMessageFieldGenerationModels.FieldData(fieldInfo), lineAction);
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
}