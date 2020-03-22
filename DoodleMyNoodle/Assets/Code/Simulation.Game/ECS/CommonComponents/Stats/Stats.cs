using Unity.Entities;
using static Unity.Mathematics.math;

internal static partial class CommonWrites
{
    public static void SetStat<T>(World world, Entity entity, T compData)
        where T : struct, IComponentData, IStatInt
    {
        if (world.EntityManager.TryGetComponentData(entity, out Minimum<T> minimum))
        {
            compData.Value = max(minimum.Value, compData.Value);
        }

        if (world.EntityManager.TryGetComponentData(entity, out Maximum<T> maximum))
        {
            compData.Value = min(maximum.Value, compData.Value);
        }

        world.EntityManager.SetComponentData(entity, compData);
    }
}

public interface IStatInt
{
    int Value { get; set; }
}

public struct Minimum<T> : IComponentData
    where T : IComponentData, IStatInt
{
    public int Value;
}

public struct Maximum<T> : IComponentData
    where T : IComponentData, IStatInt
{
    public int Value;
}
