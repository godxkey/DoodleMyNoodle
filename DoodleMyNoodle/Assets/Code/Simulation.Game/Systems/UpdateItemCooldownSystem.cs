using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class UpdateItemCooldownSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        Entities
            .ForEach((Entity item, ref ItemCooldownTimeCounter cooldownCounter) =>
            {
                cooldownCounter.Value -= deltaTime;
                if (cooldownCounter.Value <= 0)
                {
                    PostUpdateCommands.RemoveComponent<ItemCooldownTimeCounter>(item);
                }
            });

        if (HasSingleton<NewTurnEventData>())
        {
            Entities
           .ForEach((Entity item, ref ItemCooldownTurnCounter cooldownCounter) =>
           {
               cooldownCounter.Value -= 1;
               if (cooldownCounter.Value <= 0)
               {
                   PostUpdateCommands.RemoveComponent<ItemCooldownTurnCounter>(item);
               }
           });
        }
    }
}
