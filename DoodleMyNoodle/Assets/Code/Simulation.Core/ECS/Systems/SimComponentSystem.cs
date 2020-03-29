using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.Scripting;

public abstract class SimComponentSystem : ComponentSystem
{
    /// <summary>
    /// The current Time data for this system's world.
    /// </summary>
    public new ref readonly FixTimeData Time => ref SimWorld.FixTime;

    public SimulationWorld SimWorld { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();
        SimWorld = ((SimulationWorld)World);
    }
}

public abstract class SimJobComponentSystem : JobComponentSystem
{
    /// <summary>
    /// The current Time data for this system's world.
    /// </summary>
    public new ref readonly FixTimeData Time => ref SimWorld.FixTime;

    public SimulationWorld SimWorld { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();
        SimWorld = ((SimulationWorld)World);
    }
}


// should we use this instead ?

//[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
//public class CreateInSimWorldAttribute : Attribute
//{

//}