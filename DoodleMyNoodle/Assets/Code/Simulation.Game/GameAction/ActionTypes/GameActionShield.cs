using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;

public class GameActionShield : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        if (accessor.GetComponentData<GameActionRangeData>(context.Item).Value > 0)
        {
            return new UseContract(
                new GameActionParameterTile.Description(accessor.GetComponentData<GameActionRangeData>(context.Item).Value)
                {
                    IncludeSelf = false,
                    RequiresAttackableEntity = true,
                });
        }
        else
        {
            return new UseContract();
        }
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, ref ResultData resultData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            // reduce target health
            NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);
            CommonReads.FindTileActorsWithComponents<Health>(accessor, paramTile.Tile, victims);
            foreach (var victim in victims)
            {
                ShieldTarget(accessor, context.Item, victim);
            }

            return true;
        }

        ShieldTarget(accessor, context.Item, context.InstigatorPawn);
        return true;
    }

    private void ShieldTarget(ISimWorldReadWriteAccessor accessor, Entity itemEntity, Entity pawn)
    {
        int duration = accessor.GetComponentData<GameActionEffectDurationData>(itemEntity).Value;

        if (accessor.TryGetComponentData(pawn, out Invincible invincible))
        {
            accessor.AddComponentData(pawn, new Invincible() { Duration = max(duration, invincible.Duration) });
        }
        else
        {
            accessor.AddComponentData(pawn, new Invincible() { Duration = duration });
        }
    }
}

