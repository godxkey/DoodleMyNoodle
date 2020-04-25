﻿using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class GameActionMove : GameAction
{
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
                RangeFromInstigator = accessor.GetComponentData<ActionPoints>(instigatorPawn).Value,
                Filter = TileFilterFlags.Navigable | TileFilterFlags.Inoccupied
            }
        };

        return useContract;
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn, UseData useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int instigatorAP = accessor.GetComponentData<ActionPoints>(instigatorPawn).Value;
            int2 instigatorTile = roundToInt(accessor.GetComponentData<FixTranslation>(instigatorPawn).Value).xy;

            int costToMove = lengthmanhattan(paramTile.Tile - instigatorTile);

            if (costToMove > instigatorAP)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, instigatorPawn, -costToMove);

            // set destination
            accessor.SetOrAddComponentData(instigatorPawn, new Destination() { Value = fix3(paramTile.Tile, 0) });
        }
    }
}