using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class DestroyEmptyInteractableInventorySystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
        .WithAll<Interactable>()
        .WithNone<StartingInventoryItem>()
        .ForEach((Entity interactable, ref FixTranslation pos, DynamicBuffer<InventoryItemReference> inventory) =>
        {
            if (inventory.Length == 0)
            {
                PostUpdateCommands.DestroyEntity(interactable);
            }
        });
    }
}