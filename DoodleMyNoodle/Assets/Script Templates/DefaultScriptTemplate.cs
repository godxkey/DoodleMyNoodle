using System;
using System.Linq;
using UnityEngine;


public class DefaultScriptTemplate : ScriptTemplate
{
    public override string GetScriptContent()
    {
        // When refering to a type, it is recommended to use {nameof(TheType)}.
        // This helps keeping the templates up-to-date when types are renamed or removed.

        return
$@"using {nameof(System)};
using {nameof(UnityEngine)};

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
                $"using {nameof(Unity.Mathematics)};",
                $"using static {nameof(Unity.Mathematics.math)};"
            };
        }

        return null;
    }
}
