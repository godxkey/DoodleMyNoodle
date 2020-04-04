using Unity.Entities;

public static class SimWorldAccessorExtensions
{
    public static bool TryGetComponentData<T>(this ISimWorldReadAccessor accessor, Entity entity, out T componentData)
         where T : struct, IComponentData
    {
        if (accessor.HasComponent<T>(entity))
        {
            componentData = accessor.GetComponentData<T>(entity);
            return true;
        }

        componentData = default;
        return false;
    }
}