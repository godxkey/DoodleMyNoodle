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
                   new GameActionParameterTile.Description(accessor.GetComponentData<ItemRangeData>(context.Entity).Value)
                   {{
                       TileFilter = TileFlags.All,
                       IncludeSelf = false,
                       MustBeReachable = false,
                       RequiresAttackableEntity = false,

                       CustomTileActorPredicate = (tileActor, accessor) => !accessor.HasComponent<StaticTag>(tileActor),
                       CustomTilePredicate = (tilePosition, tileEntity, accessor) => !accessor.HasComponent<StaticTag>(tileEntity)
                   }});
    }}

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {{
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {{
            return true;   
        }}

        return false;
    }}

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {{
        throw new System.NotImplementedException();
    }}

    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {{
        throw new System.NotImplementedException();
    }}
}}";
    }

    public override string GetScriptDefaultName()
    {
        return "NewGameAction";
    }
}
