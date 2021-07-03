using CCC.Fix2D;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(MovementSystemGroup))]
public class UpdateNavAgentFootingSystem : SimSystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnUpdate()
    {
        TileWorld tileWorld = CommonReads.GetTileWorld(Accessor);
        Entities
            .ForEach((ref NavAgentFootingState footing, in FixTranslation fixTranslation, in PhysicsColliderBlob colliderRef, in PhysicsVelocity velocity) =>
            {
                ref Collider collider = ref colliderRef.Collider.Value;
                fix2 belowFeet = new fix2(fixTranslation.Value.x, fixTranslation.Value.y - (fix)collider.Radius - (fix)0.05);

                // IF on ladder && (already has footing on ladder || already has footing on ground)
                if (tileWorld.GetFlags(Helpers.GetTile(fixTranslation)).IsLadder
                    && (footing.Value == NavAgentFooting.Ladder || footing.Value == NavAgentFooting.Ground))
                {
                    footing.Value = NavAgentFooting.Ladder;
                }
                else if (tileWorld.GetFlags(Helpers.GetTile(belowFeet)).IsTerrain 
                    && (footing.Value == NavAgentFooting.Ground || footing.Value == NavAgentFooting.Ladder || velocity.Linear.lengthSquared < 4))
                {
                    footing.Value = NavAgentFooting.Ground;
                }
                else if (footing.Value != NavAgentFooting.AirControl)
                {
                    footing.Value = NavAgentFooting.None;
                }
            }).Run();
    }
}
