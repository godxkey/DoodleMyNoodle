using System.Collections;
using System.Collections.Generic;

public static class NetSerializationCodeGenSettings
{
    public static Dictionary<string, string> generatedSerializersPath = new Dictionary<string, string>()
    {
        { "Game", "Assets/Code/Game/Generated/NetMessageSerializers"}
        , {"mscorlib", "Assets/Plugins/CCC/Code/NetSerializer/Generated/NetMessageSerializers"}
        //, {"Examples", "Assets/Code/Examples/Generated/NetMessageSerializers"}
    };

    public const string MenuName_Generate = "Tools/Code Generation/Net Serializers/Generate";
    public const string MenuName_Clear = "Tools/Code Generation/Net Serializers/Clear";
}
