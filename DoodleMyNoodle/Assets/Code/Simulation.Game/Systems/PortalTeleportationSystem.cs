using CCC.Fix2D;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class PortalTeleportationSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .ForEach((Entity portalEntity, in Portal portalData, in FixTranslation portalPos) =>
        {
            fix2 portalPosition = portalPos.Value;
            fix2 portalNextPosition = portalData.NextPos;

            if(EntityManager.TryGetBuffer(portalEntity, out DynamicBuffer<EntitiesInsidePortalBufferData> entitiesInsidePortal)
            && EntityManager.TryGetBuffer(portalEntity, out DynamicBuffer<EntitiesTeleportedByPortalBufferData> entitiesTeleported)
            && EntityManager.TryGetBuffer(portalData.NextPortal, out DynamicBuffer<EntitiesInsidePortalBufferData> entitiesInsideNextPortal)
            && EntityManager.TryGetBuffer(portalData.NextPortal, out DynamicBuffer<EntitiesTeleportedByPortalBufferData> nextPortalTeleportedEntities))
            {
                // when entity inside a portal, if we didn't just teleported it and was not teleported by other portal, teleport
                Entities
                .ForEach((Entity entity, ref FixTranslation entityPos) =>
                {
                    if (HasComponent<PhysicsVelocity>(entity))
                    {
                        if (IsInsidePortal(entity, entitiesInsidePortal) && !WasTeleported(entity, nextPortalTeleportedEntities) && !WasTeleported(entity, entitiesTeleported))
                        {
                            int entityIndex = GetIndexOfEntity(entity, entitiesTeleported);
                            if (entityIndex == -1)
                            {
                                entitiesTeleported.Add(new EntitiesTeleportedByPortalBufferData() { entity = entity });
                                CommonWrites.RequestTeleport(Accessor, entity, portalNextPosition);
                            }
                        }
                    }
                })
                .WithoutBurst()
                .Run();

                // Update entities inside portal
                Entities
                .ForEach((Entity entity, ref FixTranslation entityPos) =>
                {
                    if (HasComponent<PhysicsVelocity>(entity))
                    {
                        fix2 pawnPosition = entityPos.Value;

                        if (IsInsidePortalPositionRange(portalPosition, pawnPosition))
                        {
                            // we're inside the portal, add us to the list if not already there
                            int entityIndex = GetIndexOfEntity(entity, entitiesInsidePortal);
                            if (entityIndex == -1)
                            {
                                entitiesInsidePortal.Add(new EntitiesInsidePortalBufferData() { entity = entity });
                            }
                        }
                        else
                        {
                            // we're not in the portal anymore, remove us if found
                            int entityIndex = GetIndexOfEntity(entity, entitiesInsidePortal);
                            if (entityIndex >= 0)
                            {
                                entitiesInsidePortal.RemoveAt(entityIndex);
                            }

                            // we exited the portal without teleporting, this means the other portal can now teleport us
                            if (!WasTeleported(entity, entitiesTeleported))
                            {
                                int entityIndexNextPortal = GetIndexOfEntity(entity, nextPortalTeleportedEntities);
                                if (entityIndexNextPortal >= 0)
                                {
                                    nextPortalTeleportedEntities.RemoveAt(entityIndexNextPortal);
                                }
                            }
                        }
                    }
                })
                .WithoutBurst()
                .Run();
            }
        })
        .WithoutBurst()
        .Run();
    }

    bool IsInsidePortal(Entity entity, DynamicBuffer<EntitiesInsidePortalBufferData> entitiesInsidePortal)
    {
        foreach (EntitiesInsidePortalBufferData entityInsidePortal in entitiesInsidePortal)
        {
            if (entityInsidePortal.entity == entity)
            {
                return true;
            }
        }

        return false;
    }

    bool WasTeleported(Entity entity, DynamicBuffer<EntitiesTeleportedByPortalBufferData> entitiesTeleported)
    {
        foreach (EntitiesTeleportedByPortalBufferData entityTeleported in entitiesTeleported)
        {
            if (entityTeleported.entity == entity)
            {
                return true;
            }
        }

        return false;
    }

    bool IsInsidePortalPositionRange(fix2 portalPosition, fix2 entityPosition)
    {
        return (entityPosition.x < (portalPosition.x + (fix)0.5)) 
                && (entityPosition.x > (portalPosition.x - (fix)0.5))
                && (entityPosition.y < (portalPosition.y + (fix)0.5)) 
                && (entityPosition.y > (portalPosition.y - (fix)0.5));
    }

    int GetIndexOfEntity(Entity entity, DynamicBuffer<EntitiesInsidePortalBufferData> entitiesInsidePortal)
    {
        for (int i = 0; i < entitiesInsidePortal.Length; i++)
        {
            EntitiesInsidePortalBufferData entityInsidePortal = entitiesInsidePortal[i];

            if (entityInsidePortal.entity == entity)
            {
                return i;
            }
        }

        return -1;
    }

    int GetIndexOfEntity(Entity entity, DynamicBuffer<EntitiesTeleportedByPortalBufferData> entitiesTeleported)
    {
        for (int i = 0; i < entitiesTeleported.Length; i++)
        {
            EntitiesTeleportedByPortalBufferData entityTeleported = entitiesTeleported[i];

            if (entityTeleported.entity == entity)
            {
                return i;
            }
        }

        return -1;
    }
}