using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;

public static partial class SimComponentViewCodeGenerator
{
    [MenuItem(SimCodeGenSettings.MenuName_Generate)]
    public static void Generate()
    {
        Generate(false);
    }

    [MenuItem(SimCodeGenSettings.MenuName_Clear)]
    public static void Clear()
    {
        Generate(true);
    }

    static void Generate(bool clear)
    {
        List<Type> netMessageTypes = SimCodeGenUtility.GetSimComponentTypesToGenerateViewFor();

        foreach (Type simType in netMessageTypes)
        {
            GenerateCode(
                simType, 
                SimCodeGenSettings.targetAssemblies[simType.Assembly.GetName().Name],
                SimCodeGenUtility.GetSimComponentViewName(simType) + ".cs", 
                clear);
        }

        AssetDatabase.Refresh();
    }


    static void GenerateCode(Type simType, string filePath, string fileName, bool clear)
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

        using (FileStream fileStream = File.Open(completePath, FileMode.Truncate))
        {
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                Model.Generate(writer, simType, clear);
            }
        }

    }
}
