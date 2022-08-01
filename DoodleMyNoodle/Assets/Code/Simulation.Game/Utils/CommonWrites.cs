using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

internal static partial class CommonWrites
{
    public static void DisableAndScheduleForDestruction(ISimGameWorldReadWriteAccessor accessor, Entity entity)
    {
        accessor.AddComponents(entity, new ComponentTypes(typeof(Disabled), typeof(ScheduledDestroyTimestamp)));
        accessor.SetComponent<ScheduledDestroyTimestamp>(entity, accessor.Time.ElapsedTime + SimulationGameConstants.DisabledEntityDestroyDelay);
    }
}