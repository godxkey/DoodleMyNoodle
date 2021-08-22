using CCC.Fix2D;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;


public struct SystemRequestHookContact : ISingletonBufferElementData
{
    public Entity HookEntity;
    public Entity ContactEntity;
}

public class UpdateHookEntitiesSystem : SimSystemBase
{
    private EntityCommandBufferSystem _ecbSytem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _ecbSytem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        HandleHookContacts();

        var ecb = _ecbSytem.CreateCommandBuffer();

        Entities
            .WithAll<PhysicsVelocity>()
            .ForEach((Entity entity, ref HookData hookData, in FixTranslation position) =>
        {
            bool destroy = false;

            // If hook is uninitialized, initialize it
            if (hookData.State == HookData.EState.Uninitialized)
            {
                hookData.StartPosition = position;
                hookData.State = HookData.EState.TravelingForward;
            }

            // If hook is traveling back (after contact), set its velocity and the contact entity
            if (hookData.State == HookData.EState.TravelingBack)
            {
                fix2 d = hookData.StartPosition - position;

                fix dLength = length(d);

                if (dLength < (fix)0.25)
                {
                    destroy = true;
                }
                else
                {
                    fix2 dir = d / dLength;

                    var newVelocity = new PhysicsVelocity(hookData.TravelBackSpeed * dir);
                    SetComponent(entity, newVelocity);
                    
                    // give same velocity to captured entity
                    if (HasComponent<PhysicsVelocity>(hookData.TouchedEntity))
                    {
                        SetComponent(hookData.TouchedEntity, newVelocity);
                    }
                }

            }

            if (destroy)
            {
                ecb.DestroyEntity(entity);
            }
        }).Run();
    }

    private void HandleHookContacts()
    {
        var hookContacts = GetSingletonBuffer<SystemRequestHookContact>();

        foreach (var item in hookContacts)
        {
            if (TryGetComponent(item.HookEntity, out HookData hookData))
            {
                if (hookData.State == HookData.EState.TravelingForward)
                {
                    hookData.State = HookData.EState.TravelingBack;
                    hookData.TouchedEntity = item.ContactEntity;
                    SetComponent(item.HookEntity, hookData);
                }
            }
        }

        hookContacts.Clear();
    }
}