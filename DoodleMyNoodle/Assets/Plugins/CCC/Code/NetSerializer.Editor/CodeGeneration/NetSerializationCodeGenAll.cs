using UnityEditor;
using UnityEngine;

public static class NetSerializationCodeGenAll
{
    [MenuItem(NetSerializationCodeGenSettings.MenuName_Generate_All)]
    public static void GenerateAll()
    {
        NetSerializerCodeGenerator.Generate();
        DynamicNetSerializationRegistryCodeGenerator.Generate();
    }

    [MenuItem(NetSerializationCodeGenSettings.MenuName_Clear_All)]
    public static void ClearAll()
    {
        NetSerializerCodeGenerator.Clear();
        DynamicNetSerializationRegistryCodeGenerator.Clear();
    }
}
