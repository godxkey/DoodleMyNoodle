using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class CooldownCounterManagementSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        Entities
            .ForEach((Entity item, ref ItemCooldownCounter cooldownCounter) =>
            {
                cooldownCounter.Value -= deltaTime;
                if (cooldownCounter.Value <= 0)
                {
                    PostUpdateCommands.RemoveComponent<ItemCooldownCounter>(item);
                }
            });
    }
}
