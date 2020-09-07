using Unity.Mathematics;
using static fixMath;
using Unity.Entities;
using Unity.Collections;

public class GameActionComboAttack : GameAction
{
    // TODO: add settings on the item itself
    const int DAMAGE = 1;
    const int AP_COST_PER_ATTACK = 1;
    const int RANGE = 1;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        var param = new GameActionParameterTile.Description(RANGE)
        {
            IncludeSelf = false,
            RequiresAttackableEntity = true
        };

        return new UseContract(param, param);
    }

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        return true;
    }
    
    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return AP_COST_PER_ATTACK;
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        // TODO Find a better way to go through all Parameters (foreach)

        int2 instigatorTile = Helpers.GetTile(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn));
        NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);

        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data firstTile))
        {
            // melee attack has a range of RANGE
            if (lengthmanhattan(firstTile.Tile - instigatorTile) > RANGE)
            {
                return;
            }

            if (accessor.GetComponentData<ActionPoints>(context.InstigatorPawn).Value < AP_COST_PER_ATTACK)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -AP_COST_PER_ATTACK);

            // find victims
            CommonReads.FindTileActorsWithComponents<Health>(accessor, firstTile.Tile, victims);
        }

        if (parameters.TryGetParameter(1, out GameActionParameterTile.Data secondTile))
        {
            // melee attack has a range of RANGE
            if (lengthmanhattan(secondTile.Tile - instigatorTile) > RANGE)
            {
                return;
            }

            if (accessor.GetComponentData<ActionPoints>(context.InstigatorPawn).Value < AP_COST_PER_ATTACK)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -AP_COST_PER_ATTACK);

            // find victims
            CommonReads.FindTileActorsWithComponents<Health>(accessor, secondTile.Tile, victims);
        }

        foreach (Entity entity in victims)
        {
            AttackEntityOnTile(accessor, context.InstigatorPawn, entity);
        }
    }

    private void AttackEntityOnTile(ISimWorldReadWriteAccessor accessor, Entity instigator, Entity entity)
    {
        CommonWrites.RequestDamageOnTarget(accessor, instigator, entity, DAMAGE);
    }

}
