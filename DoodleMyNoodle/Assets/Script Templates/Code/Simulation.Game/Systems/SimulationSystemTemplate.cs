using Unity.Mathematics;
using static Unity.Mathematics.math;
using System;
using UnityEngine;
using UnityEngineX;


public class SimulationSystemTemplate : ScriptTemplate
{
    public override string GetScriptContent()
    {
        // When refering to a type, it is recommended to use {typeof(TheType).GetPrettyName()}.
        // This helps keeping the templates up-to-date when types are renamed or removed.

        return
$@"using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class #SCRIPTNAME# : {typeof(SimSystemBase).GetPrettyName()}
{{
    protected override void OnUpdate()
    {{
    }}
}}";
    }

    public override string GetScriptDefaultName()
    {
        return "NewScriptTemplate";
    }
}
