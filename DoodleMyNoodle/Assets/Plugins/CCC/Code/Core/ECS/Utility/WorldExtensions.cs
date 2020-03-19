using Unity.Entities;

public static class WorldExtensions
{
    public static bool DestroySystem<T>(this World world) where T : ComponentSystemBase
    {
        var sys = world.GetExistingSystem<T>();
        if (sys != null)
        {
            world.DestroySystem(sys);
            return true;
        }
        else
        {
            return false;
        }
    }
}