using CCC.Fix2D;
using Unity.Entities;

public class UpdatePlayerVelocitiesSystem : SimGameSystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<PlayerGroupDataTag>();
    }

    protected override void OnUpdate()
    {
        Entity playerGroupEntity = GetSingletonEntity<PlayerGroupDataTag>();
        fix memberOffset = GetComponent<PlayerGroupSpacing>(playerGroupEntity);
        fix2 playerGroupPosition = GetComponent<FixTranslation>(playerGroupEntity);
        fix deltaTime = Time.DeltaTime;

        Entities.ForEach((ref PhysicsVelocity velocity, in FixTranslation position, in PlayerGroupMemberIndex playerGroupIndex) =>
        {
            fix2 playerDestination = playerGroupPosition + new fix2(playerGroupIndex.Value * -memberOffset);

            velocity.Linear.x = (playerDestination.x - position.Value.x) / deltaTime;
        }).Run();
    }
}
