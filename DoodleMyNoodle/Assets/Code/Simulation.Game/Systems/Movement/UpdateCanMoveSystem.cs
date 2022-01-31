using Unity.Entities;
using CCC.Fix2D;

public class UpdateCanMoveSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref CanMove canMove) =>
        {
            // todo, check with trigger in front of characters
            canMove = true;
        }).Schedule();
    }
}