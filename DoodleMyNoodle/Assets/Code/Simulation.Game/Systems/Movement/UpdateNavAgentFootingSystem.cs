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
                fix2 PosTileBeneath = new fix2(fixTranslation.Value.x, fixTranslation.Value.y - 1);

                // TODO : si t'étais ladder si on a les pieds au sol
                if ((tileWorld.GetFlags(Helpers.GetTile(fixTranslation)).IsLadder)
                || (footing.Value == NavAgentFooting.Ladder) 
                || (tileWorld.GetFlags(Helpers.GetTile(fixTranslation)).IsTerrain))
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
