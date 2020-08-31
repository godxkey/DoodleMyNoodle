using Unity.Entities;

public class DestroyAIControllersSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        // Destroy AIs that have no more pawns
        Entities
            .WithAll<AITag>()
            .ForEach((Entity controller, ref ControlledEntity pawn) =>
            {
                if(!EntityManager.Exists(pawn) || !EntityManager.HasComponent<ControllableTag>(pawn))
                {
                    PostUpdateCommands.DestroyEntity(controller);
                }
            });
    }
}
