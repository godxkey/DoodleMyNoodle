using Unity.Entities;
using static Unity.Mathematics.math;

public interface IStatInt
{
    int Value { get; set; }
}

public struct MinimumInt<T> : IComponentData
    where T : IComponentData, IStatInt
{
    public int Value;
}

public struct MaximumInt<T> : IComponentData
    where T : IComponentData, IStatInt
{
    public int Value;
}

internal static partial class CommonWrites
{
    public static void SetStat<T>(World world, Entity entity, T compData)
        where T : struct, IComponentData, IStatInt
    {
        if (world.EntityManager.TryGetComponentData(entity, out MinimumInt<T> minimum))
        {
            compData.Value = max(minimum.Value, compData.Value);
        }

        if (world.EntityManager.TryGetComponentData(entity, out MaximumInt<T> maximum))
        {
            compData.Value = min(maximum.Value, compData.Value);
        }

        world.EntityManager.SetComponentData(entity, compData);
    }
}
