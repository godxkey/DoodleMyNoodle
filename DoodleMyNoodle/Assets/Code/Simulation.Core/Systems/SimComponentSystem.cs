using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

public interface ISimSystem
{

}

public abstract class SimComponentSystemGroup : ComponentSystemGroup, ISimSystem
{

}

public abstract class SimSystemBase : CCCSystemBase, ISimSystem
{
    /// <summary>
    /// The current Time data for this system's world.
    /// </summary>
    public new ref readonly FixTimeData Time => ref World.FixTime;

    public new SimulationWorld World { get; private set; }
    public ISimWorldReadWriteAccessor Accessor { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();
        World = (SimulationWorld)base.World;
        Accessor = World.GetInternalAccessor();
    }

    protected DynamicBuffer<T> GetSingletonBuffer<T>() where T: struct, ISingletonBufferElementData
    {
        return GetBuffer<T>(GetSingletonEntity<SingletonBuffersTag>());
    }
}