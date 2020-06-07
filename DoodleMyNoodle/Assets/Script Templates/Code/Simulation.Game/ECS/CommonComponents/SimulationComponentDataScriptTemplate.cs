using System;
using UnityEngine;
using Unity.Entities;

public class SimulationComponentDataScriptTemplate : ScriptTemplate
{
    public override string GetScriptContent()
    {
        return
$@"using {nameof(Unity.Entities)};
using {nameof(Unity.Mathematics)};

public struct #SCRIPTNAME# : {nameof(IComponentData)}
{{
    
}}";
    }

    public override string GetScriptDefaultName()
    {
        return "NewComponentData";
    }
}
