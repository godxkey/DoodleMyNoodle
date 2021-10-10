using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimulationGameWorld : SimulationWorld
{
    public FixTimeData TurnTime;
    public FixTimeData RoundTime;

    public SimulationGameWorld(string name) : base(name) { }
}