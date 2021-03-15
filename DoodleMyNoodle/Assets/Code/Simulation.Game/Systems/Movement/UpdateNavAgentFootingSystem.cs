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
                if (tileWorld.GetFlags(Helpers.GetTile(fixTranslation)).IsLadder)
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
