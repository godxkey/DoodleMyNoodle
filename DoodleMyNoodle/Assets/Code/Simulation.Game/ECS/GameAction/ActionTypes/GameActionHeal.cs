using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;
using Unity.Collections;

public class GameActionHeal : GameAction
{
    const int RANGE = 3;
    const int AP_COST = 2;
    const int HEAL = 4;

    public override bool IsInstigatorValid(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.HasComponent<ActionPoints>(context.InstigatorPawn)
            && accessor.HasComponent<FixTranslation>(context.InstigatorPawn);
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        UseContract useContract = new UseContract();
        useContract.ParameterTypes = new ParameterDescription[]
        {
            new GameActionParameterTile.Description()
            {
                RangeFromInstigator = RANGE,
                Filter = TileFilterFlags.Occupied | TileFilterFlags.NotEmpty,
                IncludeSelf = true
            }
        };

        return useContract;
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            if (accessor.GetComponentData<ActionPoints>(context.InstigatorPawn).Value < AP_COST)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -AP_COST);

            // reduce target health
            NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);
            CommonReads.FindEntitiesOnTileWithComponent<Health>(accessor, paramTile.Tile, victims);
            foreach (var entity in victims)
            {
                CommonWrites.ModifyStatInt<Health>(accessor, entity, HEAL);
            }
        }
    }
}
