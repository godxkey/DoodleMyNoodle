using Unity.Entities;
using UnityEngineX;

public enum ItemUnavailablityReason
{
    None,
    NotEnoughtAP,
    InCooldown,
    NoAction,
    NoChargesLeft,
}

public struct ItemTag : IComponentData
{
}

public struct ItemSettingAPCost : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }
}

public struct ItemTimeCooldownData : IComponentData, IStatFix
{
    public fix Value;

    fix IStatFix.Value { get => Value; set => Value = value; }
}

public struct ItemCooldownTimeCounter : IComponentData
{
    public fix Value;

    public static implicit operator fix(ItemCooldownTimeCounter val) => val.Value;
    public static implicit operator ItemCooldownTimeCounter(fix val) => new ItemCooldownTimeCounter() { Value = val };
}

public struct ItemAction : IComponentData
{
    public Entity Value;

    public static implicit operator Entity(ItemAction val) => val.Value;
    public static implicit operator ItemAction(Entity val) => new ItemAction() { Value = val };
}

public struct ItemCharges : IComponentData
{
    public int Value;

    public static implicit operator int(ItemCharges val) => val.Value;
    public static implicit operator ItemCharges(int val) => new ItemCharges() { Value = val };
}

public partial class CommonReads
{
    public static bool CanUseItem(ISimWorldReadAccessor accessor, Entity actor, Entity item)
    {
        return CanUseItem(accessor, actor, item, out _);
    }

    public static bool CanUseItem(ISimWorldReadAccessor accessor, Entity actor, Entity item, out ItemUnavailablityReason debugReason)
    {
        if (accessor.TryGetComponent(item, out ItemCharges charges) && charges == 0)
        {
            debugReason = ItemUnavailablityReason.NoChargesLeft;
            return false;
        }

        int apCost = 0;
        if (accessor.TryGetComponent(item, out ItemSettingAPCost apCostComponent))
        {
            apCost = apCostComponent.Value;
        }

        if (apCost > 0)
        {
            if (accessor.TryGetComponent(actor, out ActionPoints ap))
            {
                if (apCost > ap.Value)
                {
                    debugReason = ItemUnavailablityReason.NotEnoughtAP;
                    return false;
                }
            }
            else
            {
                debugReason = ItemUnavailablityReason.NotEnoughtAP;
                return false;
            }

            if (ap <= (fix)0)
            {
                debugReason = ItemUnavailablityReason.NotEnoughtAP;
                return false;
            }
        }

        // is in cooldown?
        if (accessor.TryGetComponent(item, out ItemCooldownTimeCounter timeCooldown) &&
            timeCooldown.Value > 0)
        {
            debugReason = ItemUnavailablityReason.InCooldown;
            return false;
        }

        if (accessor.TryGetComponent(item, out ItemAction action) && action.Value == Entity.Null)
        {
            debugReason = ItemUnavailablityReason.NoAction;
            return false;
        }

        debugReason = ItemUnavailablityReason.None;
        return true;
    }
}

internal partial class CommonWrites
{
    public static bool RequestUseItem(ISimGameWorldReadWriteAccessor accessor, Entity actor, Entity item, GameAction.UseParameters parameters = null)
    {
        if (!CommonReads.CanUseItem(accessor, actor, item, out ItemUnavailablityReason unavailabilityReason))
        {
            Log.Warning($"Instigator {accessor.GetNameSafe(actor)} cannot use item {accessor.GetNameSafe(item)} because of {unavailabilityReason}");
            return false;
        }

        accessor.TryGetComponent(item, out ItemAction itemAction);

        CommonWrites.RequestExecuteGameAction(accessor, item, itemAction, parameters);

        // reduce consumable amount
        if (accessor.GetComponent<StackableFlag>(item))
        {
            CommonWrites.DecrementItem(accessor, item, actor);
        }

        // reduce instigator AP
        if (accessor.TryGetComponent(item, out ItemSettingAPCost itemActionPointCost) && itemActionPointCost.Value != 0)
        {
            CommonWrites.ModifyStatFix<ActionPoints>(accessor, actor, -itemActionPointCost.Value);
        }

        // Cooldown
        if (accessor.TryGetComponent(item, out ItemTimeCooldownData itemTimeCooldownData))
        {
            accessor.SetOrAddComponent(item, new ItemCooldownTimeCounter() { Value = itemTimeCooldownData.Value });
        }

        // reduce charges
        if (accessor.TryGetComponent(item, out ItemCharges charges))
        {
            charges.Value--;
            accessor.SetComponent(item, charges);
        }

        return true;
    }
}
