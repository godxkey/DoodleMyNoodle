using System;
using UnityEngine;
using UnityEngineX;


public class PresentationSystemScriptTemplate : ScriptTemplate
{
    public override string GetScriptContent()
    {
        // When refering to a type, it is recommended to use {typeof(TheType).GetPrettyName()}.
        // This helps keeping the templates up-to-date when types are renamed or removed.

        return
$@"using System;
using UnityEngine;
using UnityEngineX;

public class #SCRIPTNAME# : GamePresentationSystem<#SCRIPTNAME#>
{{
    protected override void OnGamePresentationUpdate()
    {{
        
    }}
}}";
    }

    public override string GetScriptDefaultName()
    {
        return "XDisplaySystem";
    }
}