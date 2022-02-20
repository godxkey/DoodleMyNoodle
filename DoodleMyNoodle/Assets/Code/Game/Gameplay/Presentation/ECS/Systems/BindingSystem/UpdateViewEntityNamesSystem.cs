#if UNITY_EDITOR
using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class UpdateViewEntityNamesSystem : ViewSystemBase
{    

    protected override void OnUpdate()
    {
        Entities
            .WithName("UpdateViewEntityNames")
            .WithoutBurst()
            .WithChangeFilter<BindedSimEntity>()
            .ForEach((Entity viewEntity, in BindedSimEntity simEntity) =>
            {
                if (SimWorldAccessor.Exists(simEntity))
                {
                    EntityManager.SetName(viewEntity, $"View_{SimWorldAccessor.GetName(simEntity)}");
                }
                else
                {
                    EntityManager.SetName(viewEntity, $"View_{(Entity)simEntity}(destroyed)");
                }
            }).Run();
    }
}
#endif