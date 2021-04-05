using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;
using CCC.Fix2D;
using System;

public class GameActionSwap : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionRangeData),
        typeof(GameActionAPCostData)
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor _, in UseContext context)
    {
        return new UseContract(
            new GameActionParameterTile.Description(_.GetComponentData<GameActionRangeData>(context.Entity).Value)
            {
                IncludeSelf = false,

                // Can swap with any non-static actor
                CustomTileActorPredicate = (tileActor, accessor) => !accessor.HasComponent<StaticTag>(tileActor)
            });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, ref ResultData resultData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            fix2 instigatorPos = accessor.GetComponentData<FixTranslation>(context.InstigatorPawn);

            // find target
            NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);

            CommonReads.FindTileActors(accessor, paramTile.Tile, victims,
                (tileActor)
                => accessor.HasComponent<FixTranslation>(tileActor)
                && !accessor.HasComponent<StaticTag>(tileActor));

            if (victims.Length > 0)
            {
                // teleport instigator to destination
                CommonWrites.RequestTeleport(accessor, context.InstigatorPawn, paramTile.Tile);
            }

            foreach (Entity entity in victims)
            {
                CommonWrites.RequestTeleport(accessor, entity, instigatorPos);
            }

            return true;
        }

        return false;
    }
}
