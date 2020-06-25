using UnityEditor;
using UnityEngine;

public static class NetSerializationCodeGenAll
{
    [MenuItem(NetSerializationCodeGenSettings.MENUNAME_GENERATE_ALL, priority = NetSerializationCodeGenSettings.MENUPRIORITY_GENERATE_ALL)]
    public static void GenerateAll()
    {
        NetSerializerCodeGenerator.Generate();
        DynamicNetSerializationRegistryCodeGenerator.Generate();
    }

    [MenuItem(NetSerializationCodeGenSettings.MENUNAME_CLEAR_ALL, priority = NetSerializationCodeGenSettings.MENUPRIORITY_CLEAR_ALL)]
    public static void ClearAll()
    {
        NetSerializerCodeGenerator.Clear();
        DynamicNetSerializationRegistryCodeGenerator.Clear();
    }
}
