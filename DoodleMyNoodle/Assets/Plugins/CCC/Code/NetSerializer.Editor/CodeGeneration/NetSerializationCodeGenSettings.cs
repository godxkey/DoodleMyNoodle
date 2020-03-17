using System.Collections;
using System.Collections.Generic;

public static class NetSerializationCodeGenSettings
{
    public static Dictionary<string, string> generatedSerializersPath = new Dictionary<string, string>()
    {
        {"Game",                "Assets/Code/Game"                      + Serializers_AssemblyGenerationFolder},
        {"mscorlib",            "Assets/Plugins/CCC/Code/NetSerializer" + Serializers_AssemblyGenerationFolder},
        {"CCC.FixMath",         "Assets/Plugins/CCC/Code/FixMath"       + Serializers_AssemblyGenerationFolder},
        {"CCC.Online",          "Assets/Plugins/CCC/Code/Online"        + Serializers_AssemblyGenerationFolder},
        {"Simulation.Core",     "Assets/Code/Simulation.Core"           + Serializers_AssemblyGenerationFolder},
        {"Simulation.Engine",   "Assets/Code/Simulation.Engine"         + Serializers_AssemblyGenerationFolder},
        {"Simulation.Game",     "Assets/Code/Simulation.Game"           + Serializers_AssemblyGenerationFolder},
        {"SimulationIO",        "Assets/Code/SimulationIO"              + Serializers_AssemblyGenerationFolder},
    };

    public const string MenuName_Generate_All                = "CodeGen/Net Serialization/Generate %#c";
    public const string MenuName_Generate_Serializers        = "CodeGen/Net Serialization/Advanced/Generate Serializers";
    public const string MenuName_Generate_Registry           = "CodeGen/Net Serialization/Advanced/Generate Dynamic Registry";
    public const string MenuName_Clear_All                   = "CodeGen/Net Serialization/Clear %#x";
    public const string MenuName_Clear_Serializers           = "CodeGen/Net Serialization/Advanced/Clear Serializers";
    public const string MenuName_Clear_Registry              = "CodeGen/Net Serialization/Advanced/Clear Dynamic Registry";

    public const string Serializers_AssemblyGenerationFolder = "/Generated/NetMessageSerializers";
    public const string Registry_FilePath                    = "Assets/Code/DynamicNetSerializerImpl/Generated";
    public const string Registry_ClassName                   = "DynamicNetSerializationRegistry";
    public const string Registry_FileName                    = Registry_ClassName + ".generated.cs";
}
