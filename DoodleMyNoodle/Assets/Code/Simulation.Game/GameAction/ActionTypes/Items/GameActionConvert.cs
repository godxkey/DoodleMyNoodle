using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;

public class GameActionConvert : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor _, in UseContext context)
    {
        return new UseContract(
            new GameActionParameterTile.Description(_.GetComponentData<ItemRangeData>(context.Entity).Value)
            {
                IncludeSelf = false,
                CustomTileActorPredicate = (tileActor, accessor) =>
                {
                    if (accessor.HasComponent<Controllable>(tileActor))
                    {
                        var pawnController = CommonReads.GetPawnController(accessor, tileActor);
                        
                        return accessor.Exists(pawnController) && accessor.HasComponent<Team>(pawnController);
                    }
                    return false;
                }
            });
    }

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        return true;
    }

    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.GetComponentData<ItemActionPointCostData>(context.Entity).Value;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            // find target
            NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);
            CommonReads.FindTileActorsWithComponents<Controllable>(accessor, paramTile.Tile, victims);
            foreach (Entity entity in victims)
            {
                Entity pawnController = CommonReads.GetPawnController(accessor, entity);
                if (pawnController != Entity.Null && accessor.TryGetComponentData(pawnController, out Team currentTeam))
                {
                    var newTeam = currentTeam.Value == 0 ? 1 : 0;

                    accessor.SetComponentData(pawnController, new Team() { Value = newTeam });
                    if (accessor.HasComponent<Converted>(pawnController))
                    {
                        accessor.RemoveComponent<Converted>(pawnController);
                    }
                    else
                    {
                        accessor.AddComponentData(pawnController, new Converted() { RemainingTurns = accessor.GetComponentData<ItemEffectDurationData>(context.Entity).Value });
                    }

                    return true;
                }
            }
        }

        return false;
    }
}
