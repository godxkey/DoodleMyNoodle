

using Unity.Collections;
using Unity.Entities;

public struct ActionOnLifetime : IBufferElementData
{
    public Entity Action;
    public fix Lifetime;
}


public class ExecuteActionOnLifetimeSystem : SimGameSystemBase
{
    private struct ActionRequest
    {

    }

    protected override void OnUpdate()
    {
        //NativeList<(Entity instigator, Entity )>

        //Entities.ForEach((DynamicBuffer<ActionOnLifetime> actionOnLifetimes, in Lifetime lifetime) =>
        //{
        //    for (int i = actionOnLifetimes.Length - 1; i >= 0; i--)
        //    {
        //        if (actionOnLifetimes[i].Lifetime < lifetime)
        //        {

        //        }
        //    }
        //}).Run();
    }
}