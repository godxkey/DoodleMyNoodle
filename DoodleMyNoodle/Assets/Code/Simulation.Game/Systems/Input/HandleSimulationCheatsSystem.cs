using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

[NetSerializable]
public class SimInputCheatKillPlayerPawn : SimCheatInput
{
    public PersistentId PlayerId;
}

[NetSerializable]
public class SimInputCheatToggleInvincible : SimCheatInput
{
    public PersistentId PlayerId; // this should be an "Entity Pawn;" in the future
}

[NetSerializable]
public class SimInputCheatDamagePlayer : SimCheatInput
{
    public PersistentId PlayerId; // this should be an "Entity Pawn;" in the future
    public int Damage;
}

[NetSerializable]
public class SimInputCheatAddAllItems : SimCheatInput
{
    public PersistentId PlayerId; // this should be an "Entity Pawn;" in the future
}

[NetSerializable]
public class SimInputCheatNextTurn : SimCheatInput
{
}

[NetSerializable]
public class SimInputCheatRemoveAllCooldowns : SimCheatInput
{
}

[NetSerializable]
public class SimInputCheatNeverEndingTurns : SimCheatInput
{
}

[NetSerializable]
public class SimInputCheatInfiniteAP : SimCheatInput
{
    public PersistentId PlayerId; // this should be an "Entity Pawn;" in the future
}

[NetSerializable]
public class SimInputCheatTeleport : SimCheatInput
{
    public PersistentId PlayerId; // this should be an "Entity Pawn;" in the future
    public fix2 Destination;
}

public struct CheatsAllItemElement : IBufferElementData
{
    public Entity ItemPrefab;

    public static implicit operator Entity(CheatsAllItemElement val) => val.ItemPrefab;
    public static implicit operator CheatsAllItemElement(Entity val) => new CheatsAllItemElement() { ItemPrefab = val };
}

[UpdateInGroup(typeof(InputSystemGroup))]
[AlwaysUpdateSystem]
public class HandleSimulationCheatsSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        foreach (var input in World.TickInputs)
        {
            if (input is SimCheatInput cheatInput)
            {
                HandleCheat(cheatInput);
            }
        }
    }

    // Add new cheat classes here!

    public void HandleCheat(SimCheatInput cheat)
    {
        switch (cheat)
        {
            case SimInputCheatKillPlayerPawn killPlayerPawn:
            {
                Entity player = CommonReads.FindPlayerEntity(Accessor, killPlayerPawn.PlayerId);

                if (EntityManager.Exists(player) &&
                    EntityManager.TryGetComponentData(player, out ControlledEntity pawn) &&
                    EntityManager.HasComponent<Health>(pawn.Value))
                {
                    CommonWrites.SetStatInt(Accessor, pawn.Value, new Health() { Value = 0 });
                }

                break;
            }

            case SimInputCheatToggleInvincible toggleInvicible:
            {
                Entity player = CommonReads.FindPlayerEntity(Accessor, toggleInvicible.PlayerId);

                if (EntityManager.Exists(player) &&
                    EntityManager.TryGetComponentData(player, out ControlledEntity pawn))
                {
                    if (EntityManager.HasComponent<Invincible>(pawn))
                    {
                        EntityManager.RemoveComponent<Invincible>(pawn);
                    }
                    else
                    {
                        EntityManager.AddComponentData(pawn, new Invincible() { Duration = 99999 });
                    }
                }
                break;
            }

            case SimInputCheatDamagePlayer damagePlayer:
            {
                Entity player = CommonReads.FindPlayerEntity(Accessor, damagePlayer.PlayerId);

                if (EntityManager.Exists(player) &&
                    EntityManager.TryGetComponentData(player, out ControlledEntity pawn))
                {
                    if (damagePlayer.Damage > 0)
                    {
                        CommonWrites.RequestDamageOnTarget(Accessor, pawn, pawn, damagePlayer.Damage);
                    }
                    else
                    {
                        CommonWrites.RequestHealOnTarget(Accessor, pawn, pawn, -damagePlayer.Damage);
                    }
                }
                break;
            }

            case SimInputCheatAddAllItems addAllItems:
            {
                Entity player = CommonReads.FindPlayerEntity(Accessor, addAllItems.PlayerId);

                if (EntityManager.Exists(player) &&
                    EntityManager.TryGetComponentData(player, out ControlledEntity pawn))
                {
                    if (HasComponent<InventoryCapacity>(pawn))
                    {
                        SetComponent<InventoryCapacity>(pawn, 999);
                    }

                    var itemBankSystem = Accessor.GetExistingSystem<GlobalItemBankSystem>();
                    var allItems = itemBankSystem.GetAllItemInstances();

                    NativeArray<(Entity item, int stack)> itemTransfers = new NativeArray<(Entity item, int stack)>(allItems.Length, Allocator.Temp);

                    for (int i = 0; i < allItems.Length; i++)
                    {
                        itemTransfers[i] = (allItems[i], 99);
                    }

                    ItemTransationBatch itemTransationBatch = new ItemTransationBatch()
                    {
                        Source = null,
                        Destination = pawn,
                        ItemsAndStacks = itemTransfers,
                        OutResults = default
                    };

                    CommonWrites.ExecuteItemTransaction(Accessor, itemTransationBatch);
                }
                break;
            }

            case SimInputCheatNextTurn nextTurn:
            {
                CommonWrites.RequestNextTurn(Accessor);
                break;
            }

            case SimInputCheatInfiniteAP infiniteAP:
            {
                Entity player = CommonReads.FindPlayerEntity(Accessor, infiniteAP.PlayerId);

                if (EntityManager.Exists(player) &&
                    EntityManager.TryGetComponentData(player, out ControlledEntity pawn))
                {
                    EntityManager.SetComponentData(pawn, new MaximumInt<ActionPoints>() { Value = 999999 });
                    EntityManager.SetComponentData(pawn, new ActionPoints() { Value = 999999 });
                }
                break;
            }

            case SimInputCheatTeleport teleport:
            {
                Entity player = CommonReads.FindPlayerEntity(Accessor, teleport.PlayerId);

                if (EntityManager.Exists(player) &&
                    EntityManager.TryGetComponentData(player, out ControlledEntity pawn))
                {
                    CommonWrites.RequestTeleport(Accessor, pawn, teleport.Destination);
                }
                break;
            }

            case SimInputCheatRemoveAllCooldowns removeAllCooldowns:
            {
                if (HasSingleton<NoCooldownTag>())
                {
                    EntityManager.DestroyEntity(GetSingletonEntity<NoCooldownTag>());
                }
                else
                {
                    Entity entity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(entity, new NoCooldownTag());
                }

                break;
            }

            case SimInputCheatNeverEndingTurns neverEndingTurns:
            {
                if (HasSingleton<NeverEndingTurnTag>())
                {
                    EntityManager.DestroyEntity(GetSingletonEntity<NeverEndingTurnTag>());
                }
                else
                {
                    Entity entity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(entity, new NeverEndingTurnTag());
                }

                break;
            }

            default:
                break;
        }
    }
}