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

public class HandleSimulationCheatsSystem : SimComponentSystem
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

            default:
                break;
        }
    }
}