using CCC.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEditor;
using UnityEngine;

public static partial class NetSerializerCodeGenerator
{
    static HashSet<Type> s_doNoRegenerate = new HashSet<Type>();
    static HashSet<Type> s_generateArrayCode = new HashSet<Type>();
    static HashSet<Type> s_generateListCode = new HashSet<Type>();
    static Dictionary<string, (FileStream file, StreamWriter writer)> s_fileStreams = new Dictionary<string, (FileStream file, StreamWriter writer)>();

    [MenuItem(NetSerializationCodeGenSettings.MENUNAME_GENERATE_SERIALIZERS, priority = NetSerializationCodeGenSettings.MENUPRIORITY_GENERATE_SERIALIZERS)]
    public static void Generate()
    {
        Generate(false);
    }

    [MenuItem(NetSerializationCodeGenSettings.MENUNAME_CLEAR_SERIALIZERS, priority = NetSerializationCodeGenSettings.MENUPRIORITY_CLEAR_SERIALIZERS)]
    public static void Clear()
    {
        Generate(true);
    }

    static void Generate(bool clear)
    {
        s_doNoRegenerate.Clear();
        s_generateArrayCode.Clear();
        s_generateListCode.Clear();

        List<Type> serializableTypes = NetSerializationCodeGenUtility.GetNetSerializableTypes();

        foreach (Type type in serializableTypes)
        {
            GenerateCode(type, clear);
        }

        foreach (Type type in s_generateArrayCode)
        {
            GenerateCode(type, clear);
        }

        foreach (Type type in s_generateListCode)
        {
            GenerateCode(type, clear);
        }

        CloseAllFileStreams();

        AssetDatabase.Refresh();
    }

    static void GenerateCode(Type type, bool clear)
    {
        if (NetSerializationCodeGenUtility.ShouldIgnoreCodeGeneration(type) || s_doNoRegenerate.Contains(type))
        {
            return;
        }

        Type elementType = type;
        if(elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(List<>))
        {
            elementType = elementType.GetGenericArguments()[0];
        }

        string assemblyName = elementType.Assembly.GetName().Name;
        if (NetSerializationCodeGenSettings.s_GeneratedSerializersPath.ContainsKey(assemblyName))
        {
            s_doNoRegenerate.Add(type);

            string folder = NetSerializationCodeGenSettings.s_GeneratedSerializersPath[assemblyName];
            string fileName = "NetSerializers"; //NetSerializationCodeGenUtility.GetSerializerNameFromType(type);
            string filePath = $"{folder}/{fileName}.generated.cs";

            // if file was already generated, append instead of truncating it
            bool appendInFile = s_fileStreams.ContainsKey(filePath);

            if (type.IsArray)
            {
                ArrayNetSerializerModel.Generate(type, filePath, clear, appendInFile);
            }
            else if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                ListNetSerializerModel.Generate(type, filePath, clear, appendInFile);
            }
            else
            {
                StaticNetSerializerModel.Generate(type, filePath, clear, appendInFile);
            }

        }
        else
        {
            Debug.LogError("No assembly->filePath association found in dictionary for " + assemblyName);
        }
    }

    static StreamWriter GetFileStream(string filePath)
    {
        if (s_fileStreams.TryGetValue(filePath, out (FileStream file, StreamWriter writer) item))
        {
            return item.writer;
        }
        else
        {
            FileX.CreateIfInexistant(filePath);

            var file = File.Open(filePath, FileMode.Truncate);
            var writer = new StreamWriter(file);
            
            writer.Flush();

            s_fileStreams.Add(filePath, (file, writer));

            return writer;
        }
    }

    static void CloseAllFileStreams()
    {
        foreach (var item in s_fileStreams)
        {
            item.Value.writer.Dispose();
            item.Value.file.Dispose();
        }

        s_fileStreams.Clear();
    }
}