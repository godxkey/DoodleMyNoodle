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
                fix2 PosTileBeneath = new fix2(fixTranslation.Value.x, fixTranslation.Value.y - (fix)0.5);

                if ((footing.Value == NavAgentFooting.Ladder)
                || ((tileWorld.GetFlags(Helpers.GetTile(fixTranslation)).IsLadder)
                && (tileWorld.GetFlags(Helpers.GetTile(PosTileBeneath)).IsTerrain)))
                {
                    footing.Value = NavAgentFooting.Ladder;
                }
                else
                {
                    // TODO
                    footing.Value = NavAgentFooting.None;
                }
            }).Run();
    }
}
