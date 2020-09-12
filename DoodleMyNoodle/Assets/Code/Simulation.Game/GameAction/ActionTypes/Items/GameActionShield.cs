using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;

public class GameActionShield : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        if(accessor.GetComponentData<ItemRangeData>(context.Entity).Value > 0) 
        {
            return new UseContract(
                new GameActionParameterTile.Description(accessor.GetComponentData<ItemRangeData>(context.Entity).Value)
                {
                    IncludeSelf = false,
                    RequiresAttackableEntity = true,
                });
        }
        else
        {
            return new UseContract(new GameActionParameterSelfTarget.Description() { });
        }
    }

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        return true;
    }

    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.GetComponentData<ItemActionPointCostData>(context.Entity).Value;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterSelfTarget.Data self))
        {
            accessor.AddComponentData(context.InstigatorPawn, new Invincible() { Duration = accessor.GetComponentData<ItemEffectDurationData>(context.Entity).Value });

            return true;
        }

        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            // reduce target health
            NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);
            CommonReads.FindTileActorsWithComponents<Health>(accessor, paramTile.Tile, victims);
            foreach (var entity in victims)
            {
                accessor.AddComponentData(entity, new Invincible() { Duration = accessor.GetComponentData<ItemEffectDurationData>(context.Entity).Value });
            }

            return true;
        }

        return false;
    }
}

