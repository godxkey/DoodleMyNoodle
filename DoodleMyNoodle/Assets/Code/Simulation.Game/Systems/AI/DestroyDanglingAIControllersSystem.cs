using Unity.Entities;

[UpdateInGroup(typeof(PreAISystemGroup))]
public class DestroyDanglingAIControllersSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        // Destroy AIs that have no more pawns
        Entities
            .WithAll<AITag>()
            .ForEach((Entity controller, ref ControlledEntity pawn) =>
            {
                if(!EntityManager.Exists(pawn) || !EntityManager.HasComponent<Controllable>(pawn))
                {
                    PostUpdateCommands.DestroyEntity(controller);
                }
            });
    }
}
