using System;
using Unity.Entities;
using UnityEngine;
using static fixMath;
using System.Collections.Generic;
using CCC.Fix2D;


public class DestroyDeadEntitiesSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        EntityManager.DestroyEntity(GetEntityQuery(typeof(DeadTag), typeof(DestroyOnDeath)));
    }
}
