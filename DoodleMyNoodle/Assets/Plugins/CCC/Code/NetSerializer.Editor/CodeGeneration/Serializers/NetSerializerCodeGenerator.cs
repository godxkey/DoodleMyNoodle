using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

public static partial class NetSerializerCodeGenerator
{
    static HashSet<Type> doNoRegenerate = new HashSet<Type>();
    static HashSet<Type> generateArrayCode = new HashSet<Type>();


    [MenuItem(NetSerializationCodeGenSettings.MenuName_Generate)]
    static void Generate()
    {
        Generate(false);
    }

    [MenuItem(NetSerializationCodeGenSettings.MenuName_Clear)]
    static void Clear()
    {
        Generate(true);
    }

    static void Generate(bool clear)
    {
        doNoRegenerate = new HashSet<Type>(pregeneratedSerializers);
        generateArrayCode.Clear();

        ReadOnlyCollection<Type> netMessageTypes = NetSerializationCodeGenUtility.GetNetSerializableTypes();

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
        if (NetSerializationCodeGenUtility.ShouldIgnoreCodeGeneration(type) || doNoRegenerate.Contains(type))
        {
            return;
        }

        string assemblyName = type.Assembly.GetName().Name;
        if (NetSerializationCodeGenSettings.generatedSerializersPath.ContainsKey(assemblyName))
        {
            doNoRegenerate.Add(type);

            if (type.IsArray)
            {
                ArrayNetSerializerModel.Generate(
                    type,
                    NetSerializationCodeGenSettings.generatedSerializersPath[assemblyName],
                    NetSerializationCodeGenUtility.GetSerializerNameFromType(type) + ".generated.cs",
                    clear);
            }
            else
            {
                StaticNetSerializerModel.Generate(
                    type,
                    NetSerializationCodeGenSettings.generatedSerializersPath[assemblyName],
                    NetSerializationCodeGenUtility.GetSerializerNameFromType(type) + ".generated.cs",
                    clear);
            }

        }
        else
        {
            Debug.LogError("No assembly->filePath association found in dictionary for " + assemblyName);
        }
    }

}