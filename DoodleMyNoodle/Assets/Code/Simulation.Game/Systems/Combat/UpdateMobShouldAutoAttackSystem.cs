using Unity.Entities;
using CCC.Fix2D;

public struct MeleeAttackerTag : IComponentData { }
public struct DropAttackerTag : IComponentData { }

public class UpdateMobShouldAutoAttackSystem : SimGameSystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<PlayerGroupDataTag>();
    }

    protected override void OnUpdate()
    {
        // _________________________________________ Melee Attacker _________________________________________ //
        Entities
            .WithAll<MeleeAttackerTag>()
            .ForEach((ref ShouldAutoAttack shouldAttack, in CanMove canMove) =>
        {
            shouldAttack = !canMove;
        }).Schedule();

        // _________________________________________ Drop Attacker _________________________________________ //
        fix2 playerGroupPosition = GetComponent<FixTranslation>(GetSingletonEntity<PlayerGroupDataTag>());
        Entities
            .WithAll<DropAttackerTag>()
            .ForEach((ref ShouldAutoAttack shouldAttack, in FixTranslation position) =>
            {
                shouldAttack = position.Value.x < playerGroupPosition.x;
            }).Schedule();
    }
}
