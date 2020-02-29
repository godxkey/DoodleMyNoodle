using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine.Scripting;

public abstract class SimComponentSystem : ComponentSystem
{
    public SimInput[] SimInputs => ((SimulationWorld)World).OngoingTickInputs;
}

public abstract class SimJobComponentSystem : JobComponentSystem
{
    public SimInput[] SimInputs => ((SimulationWorld)World).OngoingTickInputs;
}


// should we use this instead ?

//[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
//public class CreateInSimWorldAttribute : Attribute
//{

//}