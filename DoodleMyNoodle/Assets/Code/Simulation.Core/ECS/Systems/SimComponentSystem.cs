using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.Scripting;

public abstract class SimComponentSystem : ComponentSystem
{
    public SimulationWorld SimWorld { get; private set; }
    public SimInput[] SimInputs => SimWorld.OngoingTickInputs;

    protected override void OnCreate()
    {
        base.OnCreate();
        SimWorld = ((SimulationWorld)World);
    }
}

public abstract class SimJobComponentSystem : JobComponentSystem
{
    public SimulationWorld SimWorld { get; private set; }
    public SimInput[] SimInputs => ((SimulationWorld)World).OngoingTickInputs;

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