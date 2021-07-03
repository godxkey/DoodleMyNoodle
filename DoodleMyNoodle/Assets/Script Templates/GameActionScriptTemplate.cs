using Unity.Mathematics;
using static Unity.Mathematics.math;
using System;
using UnityEngine;

public class GameActionScriptTemplate : ScriptTemplate
{
    public override string GetScriptContent()
    {
        // When refering to a type, it is recommended to use {typeof(TheType).GetPrettyName()}.
        // This helps keeping the templates up-to-date when types are renamed or removed.

        return
$@"using static fixMath;
using Unity.Entities;
using Unity.Collections;

public class #SCRIPTNAME# : {nameof(GameAction)}
{{
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {{
        return new UseContract(
                   new GameActionParameterPosition.Description()
                   {{
                       MaxRangeFromInstigator = 20,
                   }});
    }}

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {{
        if (parameters.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        {{
            return true;   
        }}

        return false;
    }}

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {{
        return base.CanBeUsedInContextSpecific(accessor, context, debugReason);
    }}

    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {{
        return base.GetMinimumActionPointCost(accessor, context);
    }}
}}";
    }

    public override string GetScriptDefaultName()
    {
        return "NewGameAction";
    }
}
