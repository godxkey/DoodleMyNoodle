using CCC.Fix2D;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

[NetSerializable]
public class SimInputCheatToggleInvincible : SimCheatInput
{
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

[NetSerializable]
public class SimInputCheatPlayerSpeed : SimCheatInput
{
    public fix PlayerGroupSpeed;
}

[NetSerializable]
public class SimInputCheatTestFred : SimCheatInput
{
    public PersistentId PlayerId;
}

[NetSerializable]
public class SimInputCheatPlayerAutoAttackEnabled : SimCheatInput
{
    public bool Enabled;
}

[NetSerializable]
public class SimInputCheatPossessPawn : SimCheatInput
{
    public PersistentId PlayerId;
    public int PawnIndex;
}

[NetSerializable]
public class SimInputCheatMultiplyMobHP : SimCheatInput
{
    public fix Multiplier;
}

public struct CheatsAllItemElement : IBufferElementData
{
    public Entity ItemPrefab;

    public static implicit operator Entity(CheatsAllItemElement val) => val.ItemPrefab;
    public static implicit operator CheatsAllItemElement(Entity val) => new CheatsAllItemElement() { ItemPrefab = val };
}

public struct SingletonCheatPlayState : IComponentData
{
    public bool UseSoloPlay;
    public PersistentId SoloPlayPlayerId;
    public int SoloPlayPawnIndex;
    public bool DisableAutoAttacks;
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
                Entity groupHeader = GetSingletonEntity<PlayerGroupDataTag>();

                if (EntityManager.Exists(groupHeader))
                {
                    const int DISTANT_FUTURE = 9999999;
                    if (EntityManager.TryGetComponent(groupHeader, out InvincibleUntilTime invincibleUntilTime))
                    {
                        EntityManager.AddComponentData(groupHeader, new InvincibleUntilTime() { Time = invincibleUntilTime.Time == DISTANT_FUTURE ? 0 : DISTANT_FUTURE });
                    }
                    else
                    {
                        EntityManager.AddComponentData(groupHeader, new InvincibleUntilTime() { Time = DISTANT_FUTURE });
                    }
                }
                break;
            }

            case SimInputCheatDamageSelf damagePlayer:
            {
                Entity player = CommonReads.FindPlayerEntity(Accessor, damagePlayer.PlayerId);

                if (EntityManager.Exists(player) &&
                    EntityManager.TryGetComponent(player, out ControlledEntity pawn))
                {
                    if (damagePlayer.Damage > 0)
                    {
                        DamageRequestSettings damageRequest = new DamageRequestSettings()
                        {
                            DamageAmount = damagePlayer.Damage,
                            InstigatorSet = default,
                            IsAutoAttack = false,
                        };

                        CommonWrites.RequestDamage(Accessor, damageRequest, pawn);
                    }
                    else
                    {
                        HealRequestSettings healRequest = new HealRequestSettings()
                        {
                            HealAmount = -damagePlayer.Damage,
                            InstigatorSet = default,
                            IsAutoAttack = false,
                        };
                        CommonWrites.RequestHeal(Accessor, healRequest, pawn);
                    }
                }
                break;
            }

            case SimInputCheatAddAllItems addAllItems:
            {
                Entity player = CommonReads.FindPlayerEntity(Accessor, addAllItems.PlayerId);

                if (EntityManager.Exists(player) &&
                    EntityManager.TryGetComponent(player, out ControlledEntity pawn))
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
                    EntityManager.TryGetComponent(player, out ControlledEntity pawn))
                {
                    CommonWrites.RequestTeleport(Accessor, pawn, teleport.Destination);
                }
                break;
            }

            case SimInputCheatRemoveAllCooldowns _:
            {
                if (HasSingleton<NoCooldownSingletonTag>())
                {
                    EntityManager.DestroyEntity(GetSingletonEntity<NoCooldownSingletonTag>());
                }
                else
                {
                    Entity entity = EntityManager.CreateEntity();
                    EntityManager.AddComponentData(entity, new NoCooldownSingletonTag());
                }

                break;
            }

            case SimInputCheatImpulseSelf impulseSelf:
            {
                Entity player = CommonReads.FindPlayerEntity(Accessor, impulseSelf.PlayerId);

                if (EntityManager.Exists(player) &&
                    EntityManager.TryGetComponent(player, out ControlledEntity pawn))
                {
                    PhysicsVelocity vel = EntityManager.GetComponentData<PhysicsVelocity>(pawn);
                    vel.Linear += impulseSelf.ImpulseValue;
                    SetComponent<PhysicsVelocity>(pawn, vel);
                }
                break;
            }

            case SimInputCheatSoloPlay soloPlay:
            {
                var cheatPlayState = GetOrCreateSingleton<SingletonCheatPlayState>();
                cheatPlayState.UseSoloPlay = true;
                cheatPlayState.SoloPlayPlayerId = soloPlay.PlayerId;
                cheatPlayState.SoloPlayPawnIndex = soloPlay.PawnIndex;
                SetSingleton(cheatPlayState);
                ApplyCheatPlayState();
                break;
            }

            case SimInputCheatPlayerSpeed playerSpeed:
            {
                SetComponent<MoveSpeed>(GetSingletonEntity<PlayerGroupDataTag>(), playerSpeed.PlayerGroupSpeed);
                break;
            }

            case SimInputCheatTestFred testFred:
            {
                Entity player = CommonReads.FindPlayerEntity(Accessor, testFred.PlayerId);

                if (EntityManager.Exists(player) &&
                    EntityManager.TryGetComponent(player, out ControlledEntity pawn))
                {
                    EntityManager.AddComponentData(pawn, new DamageReceivedProcessor()
                    {
                        FunctionId = GameFunctions.GetId(DamageProcessorTest)
                    });
                }
                break;
            }

            case SimInputCheatPlayerAutoAttackEnabled enablePlayerAutoAttacks:
            {
                var cheatPlayState = GetOrCreateSingleton<SingletonCheatPlayState>();
                cheatPlayState.DisableAutoAttacks = !enablePlayerAutoAttacks.Enabled;
                SetSingleton(cheatPlayState);
                ApplyCheatPlayState();
                break;
            }

            case SimInputCheatPossessPawn possessPawn:
            {
                var cheatPlayState = GetOrCreateSingleton<SingletonCheatPlayState>();
                if (cheatPlayState.UseSoloPlay)
                {
                    cheatPlayState.SoloPlayPlayerId = possessPawn.PlayerId;
                    cheatPlayState.SoloPlayPawnIndex = possessPawn.PawnIndex;
                    SetSingleton(cheatPlayState);
                    ApplyCheatPlayState();
                }
                else
                {
                    Entity player = CommonReads.FindPlayerEntity(Accessor, possessPawn.PlayerId);

                    if (HasComponent<ControlledEntity>(player))
                    {
                        // _________________________________________ Find Pawns _________________________________________ //
                        Entity oldPawn = Entity.Null;
                        int newPawnIndex = possessPawn.PawnIndex;

                        if (EntityManager.TryGetComponent(player, out ControlledEntity pawn))
                            oldPawn = pawn;

                        Entity newPawn2 = Entity.Null; // this fixes a bad code-gen where 'soloPlayPawn' doesn't get set ...
                        Entities.ForEach((Entity entity, in PlayerGroupMemberIndex memberIndex) =>
                        {
                            if (memberIndex == newPawnIndex)
                            {
                                newPawn2 = entity;
                            }
                        }).Run();
                        var newPawn = newPawn2;

                        // _________________________________________ Update Possession _________________________________________ //
                        if (oldPawn != Entity.Null)
                        {
                            SetComponent<Controllable>(oldPawn, default);
                        }

                        if (newPawn != Entity.Null)
                        {
                            SetComponent<Controllable>(newPawn, player);
                        }

                        SetComponent<ControlledEntity>(player, newPawn);
                    }
                }
                break;
            }


            case SimInputCheatMultiplyMobHP multiplyMobHP:
            {
                var multiplier = multiplyMobHP.Multiplier;
                Entities
                    .WithAll<MobEnemyTag>()
                    .ForEach((Entity entity, ref Health hp) =>
                {
                    hp.Value *= multiplier;
                    if (HasComponent<MaximumFix<Health>>(entity))
                    {
                        var maxHP = GetComponent<MaximumFix<Health>>(entity);
                        maxHP.Value *= multiplier;
                        SetComponent(entity, maxHP);
                    }
                }).Run();
                break;
            }
        }
    }

    private void ApplyCheatPlayState()
    {
        SingletonCheatPlayState playState = GetSingleton<SingletonCheatPlayState>();
        Entity soloPlayPawn = Entity.Null;

        if (playState.UseSoloPlay)
        {
            Entity player = CommonReads.FindPlayerEntity(Accessor, playState.SoloPlayPlayerId);

            if (HasComponent<ControlledEntity>(player))
            {
                // _________________________________________ Find Pawns _________________________________________ //
                Entity currentPawn = Entity.Null;
                int newPawnIndex = playState.SoloPlayPawnIndex;

                if (EntityManager.TryGetComponent(player, out ControlledEntity pawn))
                    currentPawn = pawn;

                Entity soloPlayPawn2 = Entity.Null; // this fixes a bad code-gen where 'soloPlayPawn' doesn't get set ...
                Entities.ForEach((Entity entity, in PlayerGroupMemberIndex memberIndex) =>
                {
                    if (memberIndex == newPawnIndex)
                    {
                        soloPlayPawn2 = entity;
                    }
                }).Run();
                soloPlayPawn = soloPlayPawn2;


                // _________________________________________ Update Possession _________________________________________ //
                if (currentPawn != Entity.Null)
                {
                    SetComponent<Controllable>(currentPawn, default);
                }

                if (soloPlayPawn != Entity.Null)
                {
                    SetComponent<Controllable>(soloPlayPawn, player);
                }

                SetComponent<ControlledEntity>(player, soloPlayPawn);
            }
        }

        // _________________________________________ Disable Auto Attack on Others _________________________________________ //
        Entities.WithAll<PlayerGroupMemberIndex>()
            .ForEach((Entity entity, DynamicBuffer<InventoryItemReference> inventory) =>
            {
                var items = inventory.ToNativeArray(Allocator.Temp);
                foreach (var item in items)
                {
                    // implementation note: Keep 'EntityManager.HasComponent', otherwise it will cause an "invalidated by a structural change" exception
                    if (EntityManager.HasComponent<PeriodicActionEnabled>(item.ItemEntity))
                    {
                        bool shouldAutoAttack = !playState.DisableAutoAttacks && (!playState.UseSoloPlay || entity == soloPlayPawn);

                        if (shouldAutoAttack)
                        {
                            if (!EntityManager.HasComponent<PeriodicActionProgress>(item.ItemEntity))
                                EntityManager.AddComponent<PeriodicActionProgress>(item.ItemEntity);
                        }
                        else
                        {
                            if (EntityManager.HasComponent<PeriodicActionProgress>(item.ItemEntity))
                                EntityManager.RemoveComponent<PeriodicActionProgress>(item.ItemEntity);
                        }
                    }
                }
            }).WithoutBurst()
        .WithStructuralChanges()
        .Run();
    }

    [RegisterGameFunction]
    public static readonly GameFunction<GameFunctionDamageReceivedProcessorArg> DamageProcessorTest = (ref GameFunctionDamageReceivedProcessorArg arg) =>
    {
        arg.RemainingDamage = 0;
    };
}