using Unity.Mathematics;
using static Unity.Mathematics.math;
using System;
using UnityEngine;


public class PresentationScriptTemplate : ScriptTemplate
{
    public override string GetScriptContent()
    {
        // When refering to a type, it is recommended to use {typeof(TheType).GetPrettyName()}.
        // This helps keeping the templates up-to-date when types are renamed or removed.

        return
$@"using {nameof(System)};
using {nameof(UnityEngine)};
using {nameof(UnityEngineX)};

public class #SCRIPTNAME# : {nameof(GamePresentationBehaviour)}
{{
    protected override void OnGamePresentationUpdate()
    {{
        
    }}
}}";
    }

    public override string GetScriptDefaultName()
    {
        return "NewPresentationScript";
    }
}
