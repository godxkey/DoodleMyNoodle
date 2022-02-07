using Unity.Entities;
using UnityEngineX;

public enum ItemUnavailablityReason
{
    None,
    NotEnoughtAP,
    InCooldown
}

public partial class CommonReads
{
    public static bool CanUseItem(ISimWorldReadAccessor accessor, Entity actor, Entity item)
    {
        return CanUseItem(accessor, actor, item, out _);
    }

    public static bool CanUseItem(ISimWorldReadAccessor accessor, Entity actor, Entity item, out ItemUnavailablityReason debugReason)
    {
        int apCost = 0;
        if (accessor.TryGetComponent(actor, out ItemSettingAPCost apCostComponent))
        {
            apCost = apCostComponent.Value;
        }

        if (apCost > 0)
        {
            if (!accessor.TryGetComponent(actor, out ActionPoints ap))
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

        debugReason = ItemUnavailablityReason.None;
        return true;
    }
}

internal partial class CommonWrites
{
    public static bool UseItem(ISimGameWorldReadWriteAccessor accessor, Entity actor, Entity item, GameAction.UseParameters parameters = null)
    {
        if (!CommonReads.CanUseItem(accessor, actor, item, out ItemUnavailablityReason unavailabilityReason))
        {
            Log.Warning($"Instigator {accessor.GetNameSafe(actor)} cannot use item {accessor.GetNameSafe(item)} because of {unavailabilityReason}");
            return false;
        }

        accessor.TryGetComponent(item, out ItemAction itemAction);

        if (CommonWrites.ExecuteGameAction(accessor, actor, itemAction, parameters))
        {
            // reduce consumable amount
            if (accessor.GetComponent<StackableFlag>(item))
            {
                CommonWrites.DecrementItem(accessor, item, actor);
            }

            // reduce instigator AP
            if (accessor.TryGetComponent(item, out ItemSettingAPCost itemActionPointCost))
            {
                CommonWrites.ModifyStatFix<ActionPoints>(accessor, actor, -itemActionPointCost.Value);
            }

            // Cooldown
            if (accessor.TryGetComponent(item, out ItemTimeCooldownData itemTimeCooldownData))
            {
                accessor.SetOrAddComponent(item, new ItemCooldownTimeCounter() { Value = itemTimeCooldownData.Value });
            }

            return true;
        }
        else
        {
            return false;
        }
    }
}
