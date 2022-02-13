using Unity.Entities;
using CCC.Fix2D;

public struct MeleeAttackerTag : IComponentData { }
public struct DropAttackerTag : IComponentData { }

public class UpdateShouldAutoAttackSystem : SimGameSystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<PlayerGroupDataTag>();
    }

    protected override void OnUpdate()
    {
        // _________________________________________ Player Attacker _________________________________________ //
        Entities
            .WithAll<ItemTag>()
            .ForEach((ref ShouldAutoAttack shouldAttack) =>
            {
                shouldAttack = true;
            }).Schedule();

        // _________________________________________ Melee Attacker _________________________________________ //
        Entities
            .WithAll<MeleeAttackerTag>()
            .ForEach((ref ShouldAutoAttack shouldAttack, in CanMove canMove, in Health hp) =>
        {
            shouldAttack = !canMove && hp.Value > 0;
        }).Schedule();

        // _________________________________________ Drop Attacker _________________________________________ //
        fix2 playerGroupPosition = GetComponent<FixTranslation>(GetSingletonEntity<PlayerGroupDataTag>());
        Entities
            .WithAll<DropAttackerTag>()
            .ForEach((ref ShouldAutoAttack shouldAttack, in FixTranslation position, in Health hp) =>
            {
                shouldAttack = position.Value.x < playerGroupPosition.x && hp.Value > 0;
            }).Schedule();
    }
}
