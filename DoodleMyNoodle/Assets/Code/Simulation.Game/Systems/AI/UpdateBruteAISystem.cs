using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public struct BruteAIData : IComponentData
{
    public BruteAIState State;
    public Entity AttackTarget;
}

public enum BruteAIState
{
    Patrol,
    Attacking
}

public class UpdateBruteAISystem : SimComponentSystem
{
    private readonly fix DETECT_RANGE = 10;

    private EntityQuery _attackableGroup;

    protected override void OnCreate()
    {
        base.OnCreate();

        _attackableGroup = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Health>(),
            ComponentType.ReadOnly<ControllableTag>(),
            ComponentType.ReadOnly<FixTranslation>());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _attackableGroup.Dispose();
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((ref BruteAIData bruteData) =>
        {

        });
    }

    private void Process(Entity entity, ref BruteAIData bruteData, in fix3 position)
    {
        switch (bruteData.State)
        {
            case BruteAIState.Patrol:
                break;
            case BruteAIState.Attacking:
                break;
            default:
                break;
        }
    }

    private void Process_Patrol(Entity controller, in Team controllerTeam, ref BruteAIData bruteData, Entity brutePawn)
    {
        fix3 brutePos = EntityManager.GetComponentData<FixTranslation>(brutePawn);

        var positions = GetComponentDataFromEntity<FixTranslation>(isReadOnly: true);
        var attackableEntities = _attackableGroup.ToEntityArray(Allocator.Temp);

        Entity closest = Entity.Null;
        fix closestDist = fix.MaxValue;
        fix detectRangeSq = DETECT_RANGE * DETECT_RANGE;

        foreach (var enemy in attackableEntities)
        {
            // Find enemy controller in other teams
            Entity enemyController = CommonReads.GetPawnController(Accessor, enemy);

            if (enemyController == Entity.Null)
                continue;

            if (!EntityManager.TryGetComponentData(enemyController, out Team enemyTeam))
                continue;

            if (enemyTeam == controllerTeam)
                continue;

            // If distance is closer, record enemy
            fix dist = lengthsq(positions[enemy].Value - brutePos);
            if (dist < detectRangeSq && dist < closestDist)
            {
                closest = enemy;
            }
        }

        // Change state: Attack!
        if (closest != Entity.Null)
        {
            bruteData.AttackTarget = closest;
            bruteData.State = BruteAIState.Attacking;
        }
    }
}