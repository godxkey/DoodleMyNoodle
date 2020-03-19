using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ResetInputAccelerationSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref InputAcceleration acc) =>
        {
            acc.Value = float3(0);
        });
    }
}
