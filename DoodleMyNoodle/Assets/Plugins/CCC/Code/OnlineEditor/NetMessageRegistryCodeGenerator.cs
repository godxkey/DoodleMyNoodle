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
        ReadOnlyCollection<Type> netMessageTypes = GetNetMessageTypes();

        GenerateCode(netMessageTypes);

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Code Generation/NetMessage Registry/Clear")]
    static void Clear()
    {
        GenerateCode(new ReadOnlyCollection<Type>(new List<Type>()));

        AssetDatabase.Refresh();
    }

    static ReadOnlyCollection<Type> GetNetMessageTypes()
    {
        Func<Type, Type, int> typeComparer = (t1, t2) =>
        {
            return string.Compare(t1.FullName, t2.FullName);
        };

        ManualSortedList<Type> netMessageTypes = new ManualSortedList<Type>(typeComparer, 128);

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(NetMessage)))
                {
                    netMessageTypes.Add(type);
                }
            }
        }

        return netMessageTypes.GetInternalList();
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
                        writer.WriteLine("        ,typeof(" + GetNiceTypeName(type) + ")");
                    else
                        writer.WriteLine("        typeof(" + GetNiceTypeName(type) + ")");
                    addComma = true;
                }
                writer.WriteLine("    };");


                writer.WriteLine();


                writer.WriteLine("    public static readonly Factory<UInt16, NetMessage> factory = new Factory<UInt16, NetMessage>(new ValueTuple<UInt16, Func<NetMessage>>[]");
                writer.WriteLine("    {");
                addComma = false;
                for (int i = 0; i < netMessageTypes.Count; i++)
                {
                    if (addComma)
                        writer.WriteLine("        ,(" + i + ", ()=> new " + GetNiceTypeName(netMessageTypes[i]) + "())");
                    else
                        writer.WriteLine("        (" + i + ", ()=> new " + GetNiceTypeName(netMessageTypes[i]) + "())");
                    addComma = true;
                }
                writer.WriteLine("    });");


                writer.WriteLine("}");
            }
        }
    }


    static string GetNiceTypeName(Type type)
    {
        return type.FullName.Replace('+', '.');
    }
}