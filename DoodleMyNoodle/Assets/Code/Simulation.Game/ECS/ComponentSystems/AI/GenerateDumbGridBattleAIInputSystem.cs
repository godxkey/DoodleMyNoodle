using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;

public struct HasAlreadyPlayedTag : IComponentData
{

}

/// <summary>
/// This system generate human-like inputs for AIs with the DumbGridBattleAI tag
/// </summary>
[UpdateBefore(typeof(ExecutePawnControllerInputSystem))]
public class GenerateDumbGridBattleAIInputSystem : SimComponentSystem
{
    private EntityQuery _hasAlreadyPlayedEntityQ;

    protected override void OnCreate()
    {
        base.OnCreate();

        _hasAlreadyPlayedEntityQ = EntityManager.CreateEntityQuery(typeof(HasAlreadyPlayedTag));
    }

    protected override void OnUpdate()
    {
        // On turn change, remove the 'HasAlreadyPlayed' tag
        if (HasSingleton<NewTurnEventData>())
        {
            EntityManager.RemoveComponent<HasAlreadyPlayedTag>(_hasAlreadyPlayedEntityQ);
        }

        int currentTeam = CommonReads.GetCurrentTurnTeam(Accessor);

        Entities
            .WithAll<DumbGridBattleAITag>()
            .WithNone<HasAlreadyPlayedTag>()
            .ForEach((Entity entity, ref Team team, ref ControlledEntity controlledPawn) =>
            {
                // Can the corresponding team play ?
                if (team.Value != currentTeam)
                {
                    return;
                }

                GameAction.UseData useData = GameAction.UseData.Create(
                    new GameActionParameterTile.Data()
                    {
                        ParamIndex = 0,
                        Tile = roundToInt(pawnPos.Value).xy + int2(-1, 0)
                    });

                GameActionMove.

                DebugService.Log("Play");

                EntityManager.AddComponent<HasAlreadyPlayedTag>(entity);
            });
    }
}