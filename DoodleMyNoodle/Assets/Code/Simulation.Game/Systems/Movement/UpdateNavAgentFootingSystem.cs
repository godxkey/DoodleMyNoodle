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
            .ForEach((ref NavAgentFootingState footing, in FixTranslation fixTranslation) =>
            {
                fix2 posTileBeneath = new fix2(fixTranslation.Value.x, fixTranslation.Value.y - (fix)0.5);

                // IF on ladder && (already has footing on ladder || over terrain)
                if (tileWorld.GetFlags(Helpers.GetTile(fixTranslation)).IsLadder
                    && (footing.Value == NavAgentFooting.Ladder || tileWorld.GetFlags(Helpers.GetTile(posTileBeneath)).IsTerrain))
                {
                    footing.Value = NavAgentFooting.Ladder;
                }
                else
                {
                    // TODO: use footing 'Ground'
                    footing.Value = NavAgentFooting.None;
                }
            }).Run();
    }
}
