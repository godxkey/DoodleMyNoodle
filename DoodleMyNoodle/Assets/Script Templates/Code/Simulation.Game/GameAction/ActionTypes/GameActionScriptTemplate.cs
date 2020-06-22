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
    // TODO: add settings on the item itself
    const int AP_COST = 1;
    const int RANGE = 1;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {{
        return new UseContract(
            new GameActionParameterTile.Description()
            {{
                Filter = TileFilterFlags.Occupied | TileFilterFlags.NotEmpty,
                RangeFromInstigator = RANGE
            }});
    }}

    public override bool IsInstigatorValid(ISimWorldReadAccessor accessor, in UseContext context)
    {{
        return accessor.HasComponent<ActionPoints>(instigatorPawn)
            && accessor.HasComponent<FixTranslation>(instigatorPawn);
    }}

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseData useData)
    {{
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {{
            if (accessor.GetComponentData<ActionPoints>(instigatorPawn).Value < AP_COST)
            {{
                return;
            }}

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, instigatorPawn, -AP_COST);
        }}
    }}
}}";
    }

    public override string GetScriptDefaultName()
    {
        return "NewGameAction";
    }
}
