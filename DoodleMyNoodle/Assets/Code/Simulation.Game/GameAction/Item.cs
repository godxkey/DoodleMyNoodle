using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;

public enum ItemUnavailablityReason
{
    None,
    NotEnoughtAP,
    InCooldown,
    NoAction,
    NoChargesLeft,
}

public struct Owner : IComponentData
{
    public Entity Value;

    public static implicit operator Entity(Owner val) => val.Value;
    public static implicit operator Owner(Entity val) => new Owner() { Value = val };
}

public struct ItemTag : IComponentData
{
}

public struct SpellCooldown : IComponentData
{
    public fix Value;
}

public struct ItemCooldownTimeCounter : IComponentData
{
    public fix Value;

    public static implicit operator fix(ItemCooldownTimeCounter val) => val.Value;
    public static implicit operator ItemCooldownTimeCounter(fix val) => new ItemCooldownTimeCounter() { Value = val };
}

public struct ItemSpell : IBufferElementData
{
    public Entity Value;

    public static implicit operator Entity(ItemSpell val) => val.Value;
    public static implicit operator ItemSpell(Entity val) => new ItemSpell() { Value = val };
}

public struct ItemCurrentSpellIndex : IComponentData
{
    public int Value;

    public static implicit operator int(ItemCurrentSpellIndex val) => val.Value;
    public static implicit operator ItemCurrentSpellIndex(int val) => new ItemCurrentSpellIndex() { Value = val };
}

public struct ItemCharges : IComponentData
{
    public int Value;

    public static implicit operator int(ItemCharges val) => val.Value;
    public static implicit operator ItemCharges(int val) => new ItemCharges() { Value = val };
}

public struct ItemStartingCharges : IComponentData
{
    public int Value;

    public static implicit operator int(ItemStartingCharges val) => val.Value;
    public static implicit operator ItemStartingCharges(int val) => new ItemStartingCharges() { Value = val };
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

        // is in cooldown?
        if (accessor.TryGetComponent(item, out ItemCooldownTimeCounter timeCooldown) &&
            timeCooldown.Value > 0)
        {
            debugReason = ItemUnavailablityReason.InCooldown;
            return false;
        }

        if (CommonReads.GetItemCurrentSpell(accessor, item) == Entity.Null)
        {
            debugReason = ItemUnavailablityReason.NoAction;
            return false;
        }

        debugReason = ItemUnavailablityReason.None;
        return true;
    }

    public static Entity GetItemCurrentSpell(ISimWorldReadAccessor accessor, Entity item)
    {
        if (accessor.TryGetBufferReadOnly(item, out DynamicBuffer<ItemSpell> spells))
        {
            var spellIndex = accessor.GetComponent<ItemCurrentSpellIndex>(item);
            if (spells.IsValidIndex(spellIndex))
                return spells[spellIndex];
        }
        return Entity.Null;
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

        Entity spell = CommonReads.GetItemCurrentSpell(accessor, item);

        CommonWrites.RequestExecuteGameAction(accessor, item, spell, parameters);

        // reduce consumable amount
        if (accessor.GetComponent<StackableFlag>(item))
        {
            CommonWrites.DecrementItem(accessor, item, actor);
        }

        // Cooldown
        if (accessor.TryGetComponent(spell, out SpellCooldown spellCooldown))
        {
            accessor.SetOrAddComponent(item, new ItemCooldownTimeCounter() { Value = spellCooldown.Value });
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
