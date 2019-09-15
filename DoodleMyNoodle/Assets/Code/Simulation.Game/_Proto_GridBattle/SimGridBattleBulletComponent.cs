using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimGridBattleBulletComponent : SimComponent, ISimTickable
{
    public FixVector2 Speed;

    public void OnSimTick()
    {
        SimTransform.WorldPosition += (FixVector3)(Speed * Simulation.DeltaTime);
    }
}
