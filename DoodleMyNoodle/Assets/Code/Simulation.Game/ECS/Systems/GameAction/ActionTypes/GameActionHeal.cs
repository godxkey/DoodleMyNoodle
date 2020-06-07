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
    const int HEAL = 3;

    public override bool IsInstigatorValid(ISimWorldReadAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn)
    {
        return accessor.HasComponent<ActionPoints>(instigatorPawn)
            && accessor.HasComponent<FixTranslation>(instigatorPawn);
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn)
    {
        UseContract useContract = new UseContract();
        useContract.ParameterTypes = new ParameterDescription[]
        {
            new GameActionParameterTile.Description()
            {
                RangeFromInstigator = RANGE,
                Filter = TileFilterFlags.Occupied,
                IncludeSelf = true
            }
        };

        return useContract;
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn, UseData useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            if (accessor.GetComponentData<ActionPoints>(instigatorPawn).Value < AP_COST)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, instigatorPawn, -AP_COST);

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
