using CCC.Fix2D;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

[NetSerializable]
public class SimInputCheatToggleInvincible : SimCheatInput
{
    public PersistentId PlayerId; // this should be an "Entity Pawn;" in the future
}

[NetSerializable]
public class SimInputCheatDamageSelf : SimCheatInput
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
public class SimInputCheatRemoveAllCooldowns : SimCheatInput
{
}

[NetSerializable]
public class SimInputCheatTeleport : SimCheatInput
{
    public PersistentId PlayerId; // this should be an "Entity Pawn;" in the future
    public fix2 Destination;
}

[NetSerializable]
public class SimInputCheatImpulseSelf : SimCheatInput
{
    public PersistentId PlayerId; // this should be an "Entity Pawn;" in the future
    public fix2 ImpulseValue;
}

[NetSerializable]
public class SimInputCheatSoloPlay : SimCheatInput
{
    public PersistentId PlayerId;
    public int PawnIndex;
}

public struct CheatsAllItemElement : IBufferElementData
{
    public Entity ItemPrefab;

    public static implicit operator Entity(CheatsAllItemElement val) => val.ItemPrefab;
    public static implicit operator CheatsAllItemElement(Entity val) => new CheatsAllItemElement() { ItemPrefab = val };
}

[UpdateInGroup(typeof(InputSystemGroup))]
[AlwaysUpdateSystem]
public class HandleSimulationCheatsSystem : SimGameSystemBase
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

            case SimInputCheatDamageSelf damagePlayer:
            {
                Entity player = CommonReads.FindPlayerEntity(Accessor, damagePlayer.PlayerId);

                if (EntityManager.Exists(player) &&
                    EntityManager.TryGetComponentData(player, out ControlledEntity pawn))
                {
                    if (damagePlayer.Damage > 0)
                    {
                        CommonWrites.RequestDamage(Accessor, pawn, damagePlayer.Damage);
                    }
                    else
                    {
                        CommonWrites.RequestHeal(Accessor, pawn, -damagePlayer.Damage);
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
                    var allItems = itemBankSystem.GetAllItemPrefabs();

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

            case SimInputCheatRemoveAllCooldowns _:
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

            case SimInputCheatImpulseSelf impulseSelf:
            {
                Entity player = CommonReads.FindPlayerEntity(Accessor, impulseSelf.PlayerId);

                if (EntityManager.Exists(player) &&
                    EntityManager.TryGetComponentData(player, out ControlledEntity pawn))
                {
                    PhysicsVelocity vel = EntityManager.GetComponentData<PhysicsVelocity>(pawn);
                    vel.Linear += impulseSelf.ImpulseValue;
                    SetComponent<PhysicsVelocity>(pawn, vel);
                }
                break;
            }

            case SimInputCheatSoloPlay soloPlay:
            {
                Entity player = CommonReads.FindPlayerEntity(Accessor, soloPlay.PlayerId);

                if (!HasComponent<ControlledEntity>(player))
                    return;

                // _________________________________________ Find Pawns _________________________________________ //
                Entity currentPawn = Entity.Null;
                Entity newPawn = Entity.Null;
                int newPawnIndex = soloPlay.PawnIndex;

                if (EntityManager.TryGetComponentData(player, out ControlledEntity pawn))
                    currentPawn = pawn;

                Entities.ForEach((Entity entity, in PlayerGroupMemberIndex memberIndex) =>
                {
                    if (memberIndex == newPawnIndex)
                    {
                        newPawn = entity;
                    }
                }).Run();


                // _________________________________________ Update Possession _________________________________________ //
                if (currentPawn != Entity.Null)
                {
                    SetComponent<Controllable>(currentPawn, default);
                }

                if (newPawn != Entity.Null)
                {
                    SetComponent<Controllable>(newPawn, player);
                }

                SetComponent<ControlledEntity>(player, newPawn);

                // _________________________________________ Disable Auto Attack on Others _________________________________________ //
                Entities.WithAll<PlayerGroupMemberIndex>()
                    .ForEach((Entity entity, DynamicBuffer<InventoryItemReference> inventory) =>
                {
                    // implementation note: Keep 'EntityManager.HasComponent', otherwise it will cause an "invalidated by a structural change" exception
                    var items = inventory.ToNativeArray(Allocator.Temp);
                    if (entity != newPawn)
                    {
                        foreach (var item in items)
                        {
                            if (EntityManager.HasComponent<AutoAttackProgress>(item.ItemEntity))
                                EntityManager.RemoveComponent<AutoAttackProgress>(item.ItemEntity);
                        }
                    }
                    else
                    {
                        foreach (var item in items)
                        {
                            if (!EntityManager.HasComponent<AutoAttackProgress>(item.ItemEntity) && EntityManager.HasComponent<ShouldAutoAttack>(item.ItemEntity))
                                EntityManager.AddComponent<AutoAttackProgress>(item.ItemEntity);
                        }
                    }
                }).WithoutBurst()
                .WithStructuralChanges()
                .Run();

                break;
            }
        }
    }
}