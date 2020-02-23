using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SimInputSystem : ComponentSystem
{
    public SimInput[] TickInputs;

    protected override void OnUpdate() { }
}
