using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

[NetSerializable]
public class SimInputCheatKillPlayerPawn : SimCheatInput
{
    public PersistentId PlayerId;
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
            default:
                break;
        }
    }
}