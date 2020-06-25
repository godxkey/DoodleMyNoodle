using System.Collections;
using System.Collections.Generic;

public static class NetSerializationCodeGenSettings
{
    public static readonly Dictionary<string, string> s_GeneratedSerializersPath = new Dictionary<string, string>()
    {
        {"Game",                "Assets/Code/Game"                      + SERIALIZERS_ASSEMBLYGENERATIONFOLDER},
        {"mscorlib",            "Assets/Plugins/CCC/Code/NetSerializer" + SERIALIZERS_ASSEMBLYGENERATIONFOLDER},
        {"CCC.FixMath",         "Assets/Plugins/CCC/Code/FixMath"       + SERIALIZERS_ASSEMBLYGENERATIONFOLDER},
        {"CCC.Online",          "Assets/Plugins/CCC/Code/Online"        + SERIALIZERS_ASSEMBLYGENERATIONFOLDER},
        {"Simulation.Core",     "Assets/Code/Simulation.Core"           + SERIALIZERS_ASSEMBLYGENERATIONFOLDER},
        {"Simulation.Engine",   "Assets/Code/Simulation.Engine_OLD"     + SERIALIZERS_ASSEMBLYGENERATIONFOLDER},
        {"Simulation.Game",     "Assets/Code/Simulation.Game"           + SERIALIZERS_ASSEMBLYGENERATIONFOLDER},
        {"SimulationIO",        "Assets/Code/SimulationIO"              + SERIALIZERS_ASSEMBLYGENERATIONFOLDER},
    };

    public const string MENUNAME_GENERATE_ALL                = "Tools/CodeGen/Net Serialization/Generate %#c";
    public const string MENUNAME_GENERATE_SERIALIZERS        = "Tools/CodeGen/Net Serialization/Advanced/Generate Serializers";
    public const string MENUNAME_GENERATE_REGISTRY           = "Tools/CodeGen/Net Serialization/Advanced/Generate Dynamic Registry";
    public const string MENUNAME_CLEAR_ALL                   = "Tools/CodeGen/Net Serialization/Clear %#x";
    public const string MENUNAME_CLEAR_SERIALIZERS           = "Tools/CodeGen/Net Serialization/Advanced/Clear Serializers";
    public const string MENUNAME_CLEAR_REGISTRY              = "Tools/CodeGen/Net Serialization/Advanced/Clear Dynamic Registry";
    
    public const int MENUPRIORITY_GENERATE_ALL                = 12;
    public const int MENUPRIORITY_GENERATE_SERIALIZERS        = 13;
    public const int MENUPRIORITY_GENERATE_REGISTRY           = 14;
    public const int MENUPRIORITY_CLEAR_ALL                   = 15;
    public const int MENUPRIORITY_CLEAR_SERIALIZERS           = 16;
    public const int MENUPRIORITY_CLEAR_REGISTRY              = 17;

    public const string SERIALIZERS_ASSEMBLYGENERATIONFOLDER = "/Generated/NetMessageSerializers";
    public const string REGISTRY_FILEPATH                    = "Assets/Code/DynamicNetSerializerImpl/Generated";
    public const string REGISTRY_CLASSNAME                   = "DynamicNetSerializationRegistry";
    public const string REGISTRY_FILENAME                    = REGISTRY_CLASSNAME + ".generated.cs";
}
