using System.Collections;
using System.Collections.Generic;

public static class NetSerializationCodeGenSettings
{
    public static Dictionary<string, string> generatedSerializersPath = new Dictionary<string, string>()
    {
        { "Game", "Assets/Code/Game" + Serializers_AssemblyGenerationFolder}
        , {"mscorlib", "Assets/Plugins/CCC/Code/NetSerializer" + Serializers_AssemblyGenerationFolder}
        , {"CCC.FixMath", "Assets/Plugins/CCC/Code/FixMath" + Serializers_AssemblyGenerationFolder}
        , {"Simulation.Core", "Assets/Code/Simulation.Core" + Serializers_AssemblyGenerationFolder}
        , {"Simulation.Base", "Assets/Code/Simulation.Base" + Serializers_AssemblyGenerationFolder}
        , {"Simulation.Impl", "Assets/Code/Simulation.Impl" + Serializers_AssemblyGenerationFolder}
        //, {"Examples", "Assets/Code/Examples/Generated/NetMessageSerializers"}
    };

    public const string MenuName_Generate_All                = "Tools/Code Generation/Net Serialization/Generate";
    public const string MenuName_Generate_Serializers        = "Tools/Code Generation/Net Serialization/Advanced/Generate Serializers";
    public const string MenuName_Generate_Registry           = "Tools/Code Generation/Net Serialization/Advanced/Generate Dynamic Registry";
    public const string MenuName_Clear_All                   = "Tools/Code Generation/Net Serialization/Clear";
    public const string MenuName_Clear_Serializers           = "Tools/Code Generation/Net Serialization/Advanced/Clear Serializers";
    public const string MenuName_Clear_Registry              = "Tools/Code Generation/Net Serialization/Advanced/Clear Dynamic Registry";

    public const string Serializers_AssemblyGenerationFolder = "/Generated/NetMessageSerializers";
    public const string Registry_FilePath                    = "Assets/Code/DynamicNetSerializerImpl/Generated";
    public const string Registry_ClassName                   = "DynamicNetSerializationRegistry";
    public const string Registry_FileName                    = Registry_ClassName + ".generated.cs";
}
