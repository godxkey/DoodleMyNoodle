using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;

public static class NetMessageRegistryCodeGenerator
{
    const string FilePath = "Assets/Code/Game/Generated";
    const string FileName = "NetMessageRegistry.generated.cs";
    static readonly string CompletePath = FilePath + '/' + FileName;


    [MenuItem("Tools/Code Generation/NetMessage Registry/Generate")]
    static void Generate()
    {
        GenerateCode(NetMessageCodeGenUtility.GetNetMessageTypes());

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Code Generation/NetMessage Registry/Clear")]
    static void Clear()
    {
        GenerateCode(new ReadOnlyCollection<Type>(new List<Type>()));

        AssetDatabase.Refresh();
    }

    static ulong GetHashFromNetMessageTypes(ReadOnlyCollection<Type> netMessageTypes)
    {
        StringBuilder concatenatedNamesBuilder = new StringBuilder();
        foreach (var t in netMessageTypes)
        {
            concatenatedNamesBuilder.Append(t.FullName);
        }

        string concatenatedNames = concatenatedNamesBuilder.ToString();
        byte[] crcData = new byte[concatenatedNames.Length * 2]; // each char is 2 bytes

        BitStreamWriter writer = new BitStreamWriter(crcData);
        for (int i = 0; i < concatenatedNames.Length; i++)
        {
            writer.WriteChar(concatenatedNames[i]);
        }

        return Crc64.Compute(crcData);
    }

    static void GenerateCode(ReadOnlyCollection<Type> netMessageTypes)
    {
        ulong crc = GetHashFromNetMessageTypes(netMessageTypes);

        if (!Directory.Exists(FilePath))
        {
            Directory.CreateDirectory(FilePath);
        }

        if (!File.Exists(CompletePath))
        {
            File.Create(CompletePath).Close();
        }

        using (FileStream fileStream = File.Open(CompletePath, FileMode.Truncate))
        {
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Flush();

                writer.WriteLine("// THIS CODE IS GENERATED");
                writer.WriteLine("// DO NOT MODIFY IT");
                writer.WriteLine();
                writer.WriteLine("using System;");
                writer.WriteLine("using System.Collections.Generic;");
                writer.WriteLine();
                writer.WriteLine("public static class NetMessageRegistry");
                writer.WriteLine("{");

                writer.WriteLine("    public static readonly ulong crc = " + crc + ";");

                writer.WriteLine();

                writer.WriteLine("    public static readonly Type[] types = new Type[]");
                writer.WriteLine("    {");
                bool addComma = false;
                foreach (Type type in netMessageTypes)
                {
                    if (addComma)
                        writer.WriteLine("        ,");
                    writer.WriteLine("        typeof(" + GetNiceTypeName(type) + ")");
                    addComma = true;
                }
                writer.WriteLine("    };");


                writer.WriteLine();


                writer.WriteLine("    public static readonly Dictionary<UInt16, Func<object, int>> netBitSizeMap = new Dictionary<UInt16, Func<object, int>>()");
                writer.WriteLine("    {");
                addComma = false;
                for (int i = 0; i < netMessageTypes.Count; i++)
                {
                    Type t = netMessageTypes[i];

                    if (addComma)
                        writer.WriteLine("        ,");
                    writer.WriteLine("        [" + i + "] = (obj) =>");
                    writer.WriteLine("        {");
                    writer.WriteLine("            " + GetNiceTypeName(t) + " castedObj = (" + GetNiceTypeName(t) + ")obj;");
                    writer.WriteLine("            return " + NetMessageCodeGenUtility.GetSerializerNameFromType(t) + ".GetNetBitSize(ref castedObj);");
                    writer.WriteLine("        }");
                    addComma = true;
                }
                writer.WriteLine("    };");


                writer.WriteLine();


                writer.WriteLine("    public static readonly Dictionary<UInt16, Action<object, BitStreamWriter>> serializationMap = new Dictionary<UInt16, Action<object, BitStreamWriter>>()");
                writer.WriteLine("    {");
                addComma = false;
                for (int i = 0; i < netMessageTypes.Count; i++)
                {
                    Type t = netMessageTypes[i];

                    if (addComma)
                        writer.WriteLine("        ,");
                    writer.WriteLine("        [" + i + "] = (obj, writer) =>");
                    writer.WriteLine("        {");
                    writer.WriteLine("            " + GetNiceTypeName(t) + " castedObj = (" + GetNiceTypeName(t) + ")obj;");
                    writer.WriteLine("            " + NetMessageCodeGenUtility.GetSerializerNameFromType(t) + ".NetSerialize(ref castedObj, writer);");
                    writer.WriteLine("        }");
                    addComma = true;
                }
                writer.WriteLine("    };");


                writer.WriteLine();


                writer.WriteLine("    public static readonly Dictionary<UInt16, Func<BitStreamReader, object>> deserializationMap = new Dictionary<UInt16, Func<BitStreamReader, object>>()");
                writer.WriteLine("    {");
                addComma = false;
                for (int i = 0; i < netMessageTypes.Count; i++)
                {
                    Type t = netMessageTypes[i];

                    if (addComma)
                        writer.WriteLine("        ,");
                    writer.WriteLine("        [" + i + "] = (reader) =>");
                    writer.WriteLine("        {");
                    writer.WriteLine("            " + GetNiceTypeName(t) + " obj = default;");
                    writer.WriteLine("            " + NetMessageCodeGenUtility.GetSerializerNameFromType(t) + ".NetDeserialize(ref obj, reader);");
                    writer.WriteLine("            return obj;");
                    writer.WriteLine("        }");
                    addComma = true;
                }
                writer.WriteLine("    };");


                writer.WriteLine("}");
            }
        }
    }


    static string GetNiceTypeName(Type type)
    {
        return type.Name; //return type.FullName.Replace('+', '.');
    }
}