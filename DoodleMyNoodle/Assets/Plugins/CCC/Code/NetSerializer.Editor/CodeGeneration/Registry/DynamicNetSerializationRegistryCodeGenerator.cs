using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngineX;
using UnityEditor;

public static class DynamicNetSerializationRegistryCodeGenerator
{
    static readonly string CompletePath = NetSerializationCodeGenSettings.REGISTRY_FILEPATH + '/' + NetSerializationCodeGenSettings.REGISTRY_FILENAME;


    [MenuItem(NetSerializationCodeGenSettings.MENUNAME_GENERATE_REGISTRY, priority = NetSerializationCodeGenSettings.MENUPRIORITY_GENERATE_REGISTRY)]
    public static void Generate()
    {
        var types = NetSerializationCodeGenUtility.GetNetSerializableTypes();
        types.RemoveAll((t) => t.IsAbstract);
        GenerateCode(types.ToArray());

        AssetDatabase.Refresh();
    }

    [MenuItem(NetSerializationCodeGenSettings.MENUNAME_CLEAR_REGISTRY, priority = NetSerializationCodeGenSettings.MENUPRIORITY_CLEAR_REGISTRY)]
    public static void Clear()
    {
        GenerateCode(new Type[0]);

        AssetDatabase.Refresh();
    }

    static ulong GetHashFromNetMessageTypes(Type[] netMessageTypes)
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

    static void GenerateCode(Type[] netMessageTypes)
    {
        ulong crc = GetHashFromNetMessageTypes(netMessageTypes);

        if (!Directory.Exists(NetSerializationCodeGenSettings.REGISTRY_FILEPATH))
        {
            Directory.CreateDirectory(NetSerializationCodeGenSettings.REGISTRY_FILEPATH);
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
                writer.WriteLine("public static class " + NetSerializationCodeGenSettings.REGISTRY_CLASSNAME);
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
                    writer.WriteLine("        typeof(" + type.GetPrettyFullName() + ")");
                    addComma = true;
                }
                writer.WriteLine("    };");


                writer.WriteLine();


                writer.WriteLine("    public static readonly Dictionary<Type, Func<object, int>> map_GetBitSize = new Dictionary<Type, Func<object, int>>()");
                writer.WriteLine("    {");
                addComma = false;
                for (int i = 0; i < netMessageTypes.Length; i++)
                {
                    Type t = netMessageTypes[i];

                    if (addComma)
                        writer.WriteLine("        ,");
                    writer.WriteLine("        [typeof(" + t.GetPrettyFullName() + ")] = (obj) =>");
                    writer.WriteLine("        {");
                    writer.WriteLine("            " + t.GetPrettyFullName() + " castedObj = (" + t.GetPrettyFullName() + ")obj;");
                    if (NetSerializationCodeGenUtility.ConsideredAsValueType(t))
                        writer.WriteLine("            return " + NetSerializationCodeGenUtility.GetSerializerNameFromType(t) + ".GetSerializedBitSize(ref castedObj);");
                    else
                        writer.WriteLine("            return " + NetSerializationCodeGenUtility.GetSerializerNameFromType(t) + ".GetSerializedBitSize(castedObj);");
                    writer.WriteLine("        }");
                    addComma = true;
                }
                writer.WriteLine("    };");


                writer.WriteLine();


                writer.WriteLine("    public static readonly Dictionary<Type, Action<object, BitStreamWriter>> map_Serialize = new Dictionary<Type, Action<object, BitStreamWriter>>()");
                writer.WriteLine("    {");
                addComma = false;
                for (int i = 0; i < netMessageTypes.Length; i++)
                {
                    Type t = netMessageTypes[i];

                    if (addComma)
                        writer.WriteLine("        ,");
                    writer.WriteLine("        [typeof(" + t.GetPrettyFullName() + ")] = (obj, writer) =>");
                    writer.WriteLine("        {");
                    writer.WriteLine("            " + t.GetPrettyFullName() + " castedObj = (" + t.GetPrettyFullName() + ")obj;");
                    if (NetSerializationCodeGenUtility.ConsideredAsValueType(t))
                        writer.WriteLine("            " + NetSerializationCodeGenUtility.GetSerializerNameFromType(t) + ".Serialize(ref castedObj, writer);");
                    else
                        writer.WriteLine("            " + NetSerializationCodeGenUtility.GetSerializerNameFromType(t) + ".Serialize(castedObj, writer);");
                    writer.WriteLine("        }");
                    addComma = true;
                }
                writer.WriteLine("    };");


                writer.WriteLine();


                writer.WriteLine("    public static readonly Dictionary<UInt16, Func<BitStreamReader, object>> map_Deserialize = new Dictionary<UInt16, Func<BitStreamReader, object>>()");
                writer.WriteLine("    {");
                addComma = false;
                for (int i = 0; i < netMessageTypes.Length; i++)
                {
                    Type t = netMessageTypes[i];

                    if (addComma)
                        writer.WriteLine("        ,");
                    writer.WriteLine("        [" + i + "] = (reader) =>");
                    writer.WriteLine("        {");
                    writer.WriteLine("            " + t.GetPrettyFullName() + " obj = new " + t.GetPrettyFullName() + "();");
                    if (NetSerializationCodeGenUtility.ConsideredAsValueType(t))
                        writer.WriteLine("            " + NetSerializationCodeGenUtility.GetSerializerNameFromType(t) + ".Deserialize(ref obj, reader);");
                    else
                        writer.WriteLine("            " + NetSerializationCodeGenUtility.GetSerializerNameFromType(t) + ".Deserialize(obj, reader);");
                    writer.WriteLine("            return obj;");
                    writer.WriteLine("        }");
                    addComma = true;
                }
                writer.WriteLine("    };");


                writer.WriteLine("}");
            }
        }
    }
}