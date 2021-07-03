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

                // GOTO Ladder IF on ladder && (already has footing on ladder || already has footing on ground)
                if (tileWorld.GetFlags(Helpers.GetTile(fixTranslation)).IsLadder
                    && (footing.Value == NavAgentFooting.Ladder || footing.Value == NavAgentFooting.Ground))
                {
                    footing.Value = NavAgentFooting.Ladder;
                }

                // GOTO Ground IF above terrain && (previously grounded || previously ladder || previously airControl and not jumping || velocity is low)
                else if (tileWorld.GetFlags(Helpers.GetTile(belowFeet)).IsTerrain 
                    && (footing.Value == NavAgentFooting.Ground 
                        || footing.Value == NavAgentFooting.Ladder
                        || (footing.Value == NavAgentFooting.AirControl && velocity.Linear.y <= (fix)0.5)
                        || velocity.Linear.lengthSquared < 4))
                {
                    footing.Value = NavAgentFooting.Ground;
                }

                // GOTO air control IF in mid-air && was not in None
                else if (footing.Value != NavAgentFooting.None)
                {
                    footing.Value = NavAgentFooting.AirControl;
                }
            }).Run();

        // When pawn is in the air, reduce friction so we don't break on walls
        Entities
            .WithChangeFilter<NavAgentFootingState>()
            .ForEach((ref PhysicsColliderBlob collider, in NavAgentFootingState footing, in NonAirControlFriction nonAirControlFriction) =>
            {
                var material = collider.Collider.Value.Material;
                material.Friction = footing.Value == NavAgentFooting.AirControl ? 0 : (float)nonAirControlFriction.Value;
                collider.Collider.Value.Material = material;
            }).Run();
    }
}
