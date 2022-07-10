using Unity.Entities;
using CCC.Fix2D;
using Unity.Mathematics;
using static fixMath;

[UpdateInGroup(typeof(MovementSystemGroup))]
[UpdateBefore(typeof(UpdateMovementSystem))]
public partial class UpdateCanMoveSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        if (!HasSingleton<GameStartedTag>())
        {
            Entities
                .ForEach((ref CanMove canMove) =>
                {
                    canMove = false;
                }).Schedule();
            return;
        }

        Entities
            .ForEach((ref CanMove canMove, in Health hp, in Grounded grounded) =>
            {
                canMove =
                    // entities need to be alive to move
                    hp.Value > 0

                    // entities need to be grounded (nb: flying entities do not have the component)
                    && grounded;
            }).Schedule();

        Entities
            .ForEach((ref CanMove canMove, in Health hp, in Grounded grounded, in Flying flying) =>
            {
                canMove =
                    // entities need to be alive to move
                    hp.Value > 0

                    // entities need to be grounded unless they're flying
                    && (grounded || flying);
            }).Schedule();

        Entities
            .WithNone<Grounded>()
            .ForEach((ref CanMove canMove, in Health hp) =>
            {
                canMove =
                    // entities need to be alive to move
                    hp.Value > 0;
            }).Schedule();
    }
}