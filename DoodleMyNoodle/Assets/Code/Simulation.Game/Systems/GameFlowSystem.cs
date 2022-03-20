using CCC.Fix2D;
using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public struct GameStartedTag : IComponentData { }

public struct ReadyToPlay : IComponentData
{
    public bool Value;

    public static implicit operator bool(ReadyToPlay val) => val.Value;
    public static implicit operator ReadyToPlay(bool val) => new ReadyToPlay() { Value = val };
}

public class GameFlowSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        StartGameIfNeeded();
        EndGameIfNeeded();
    }

    private void StartGameIfNeeded()
    {
        if (HasSingleton<GameStartedTag>())
            return;

        bool everyoneIsReady = true;
        bool atLeastOnePlayerExists = false;

        // check if every player is ready
        Entities
            .ForEach((in Active active, in ReadyToPlay readyToPlay, in ControlledEntity pawn) =>
        {
            if (active)
            {
                atLeastOnePlayerExists = true;

                if (!readyToPlay && HasComponent<Controllable>(pawn))
                {
                    everyoneIsReady = false; // if a team member is NOT ready
                }
            }
        }).Run();

        if (atLeastOnePlayerExists && everyoneIsReady)
        {
            CreateSingleton<GameStartedTag>();
        }
    }

    private void EndGameIfNeeded()
    {
        // Upon new turn, check if a team is empty
        if (HasSingleton<GameStartedTag>())
        {
            if (TryGetSingletonEntity<PlayerGroupDataTag>(out Entity playerGroup))
            {
                // Team HP is empty, they're dead = they lost :(
                if (EntityManager.TryGetComponent(playerGroup, out Health health))
                {
                    if (health.Value <= 0)
                    {
                        // Ennemy win !
                        if (!HasSingleton<WinningTeam>())
                        {
                            CreateSingleton(new WinningTeam { Value = (int)DesignerFriendlyTeam.Baddies });
                        }
                    }
                }

                // Team reached their destination for this level = they win !
                if (EntityManager.TryGetComponent(playerGroup, out FixTranslation translation))
                {
                    if(TryGetSingleton(out GameOverDestinationToReachSingleton gameOverDestinationToReachSingleton)) 
                    {
                        if (translation.Value.x >= gameOverDestinationToReachSingleton.XPosition)
                        {
                            // Player wins !
                            if (!HasSingleton<WinningTeam>())
                            {
                                CreateSingleton(new WinningTeam { Value = (int)DesignerFriendlyTeam.Player });
                            }
                        }
                    }
                }
            }
        }
    }
}