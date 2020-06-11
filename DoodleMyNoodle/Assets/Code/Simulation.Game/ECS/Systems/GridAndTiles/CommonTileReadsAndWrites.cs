using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Collections;
using Unity.Entities;

public partial class CommonReads
{
    public static fix3 GetFloorPlaneNormal(ISimWorldReadAccessor accessor) 
    {
        return fix3(0, 0, -1);
    }

    public static void FindEntitiesOnTileWithComponent<T>(ISimWorldReadAccessor accessor, int2 tile, NativeList<Entity> result)
        where T : struct, IComponentData
    {
        accessor.Entities
            .WithAll<T>()
            .ForEach((Entity entity, ref FixTranslation pos) =>
            {
                if (Helpers.GetTile(pos).Equals(tile))
                {
                    result.Add(entity);
                }
            });
    }

    public static void FindEntitiesOnTileWithComponents<T1, T2>(ISimWorldReadAccessor accessor, int2 tile, NativeList<Entity> result)
        where T1 : struct, IComponentData
        where T2 : struct, IComponentData
    {
        accessor.Entities
            .WithAll<T1, T2>()
            .ForEach((Entity entity, ref FixTranslation pos) =>
        {
            if(Helpers.GetTile(pos).Equals(tile))
            {
                result.Add(entity);
            }
        });
    }
}

public partial class Helpers
{
    public static int2 GetTile(in FixTranslation translation) => roundToInt(translation.Value).xy;
    public static int2 GetTile(in fix3 worldPosition) => roundToInt(worldPosition).xy;
    public static fix3 GetTileCenter(in FixTranslation translation) => fix3(roundToInt(translation.Value).xy, 0);
    public static fix3 GetTileCenter(in fix3 worldPosition) => fix3(roundToInt(worldPosition).xy, 0);
    public static fix3 GetTileCenter(in int2 tile) => fix3(tile, 0);
}