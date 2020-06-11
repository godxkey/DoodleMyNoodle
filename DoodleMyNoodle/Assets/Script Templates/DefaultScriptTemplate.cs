using System;
using System.Linq;
using UnityEngine;
using UnityEngineX;

public class DefaultScriptTemplate : ScriptTemplate
{
    public override string GetScriptContent()
    {
        // When refering to a type, it is recommended to use {typeof(TheType).GetPrettyName()}.
        // This helps keeping the templates up-to-date when types are renamed or removed.

        return
$@"using {nameof(System)};
using {nameof(UnityEngine)};
using {nameof(UnityEngineX)};

public class #SCRIPTNAME# : {nameof(MonoBehaviour)}
{{
    void Start()
    {{

    }}
    
    void Update()
    {{

    }}
}}";
    }

    public override string GetScriptDefaultName()
    {
        return "NewScript";
    }


    [DefaultSmartScriptResolver.AdditionalUsingsProvider]
    public static string[] GetAdditionalIncludes(DefaultSmartScriptResolver.Info info)
    {
        if (info.Assembly.GetReferencedAssemblies().Any((asm) => asm.Name == "Unity.Mathematics"))
        {
            return new string[]
            {
                $"using Unity.Mathematics;",
                $"using static {typeof(Unity.Mathematics.math).GetPrettyFullName()};"
            };
        }

        return null;
    }
}
