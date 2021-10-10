using CCC.Fix2D;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class DispenserSpawningEntitiesSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity, ref EntitySpawnerState entitySpawnerState, in EntitySpawnerSetting entitySpawnerSetting, in DynamicBuffer<EntitiesToSpawn> entitiesToSpawn, in Signal signal, in FixTranslation translation) =>
        {
            if (entitySpawnerState.TotalAmountSpawned < entitySpawnerSetting.Quantity)
            {
                if (!entitySpawnerSetting.OnlyWhenSignalOn || signal.Value)
                {
                    NativeArray<EntitiesToSpawn> entitiesToSpawnList = entitiesToSpawn.ToNativeArray(Allocator.Temp);

                    if (!(entitySpawnerSetting.SpawnPeriod.Type == TimeValue.ValueType.Seconds))
                    {
                        if (HasSingleton<NewTurnEventData>())
                        {
                            if (entitySpawnerSetting.SpawnPeriod.Type == TimeValue.ValueType.Rounds && GetSingleton<TurnCurrentTeamSingletonComponent>().Value == 0)
                            {
                                entitySpawnerState.TrackedTime.Value++;

                                if (entitySpawnerState.TrackedTime.Value >= entitySpawnerSetting.SpawnPeriod.Value)
                                {
                                    entitySpawnerState.TrackedTime.Value = 0;

                                    SpawnEntities(entitiesToSpawnList, entitySpawnerSetting, ref entitySpawnerState, translation);
                                }
                                
                            }
                            else if (entitySpawnerSetting.SpawnPeriod.Type == TimeValue.ValueType.Turns)
                            {
                                entitySpawnerState.TrackedTime.Value++;

                                if (entitySpawnerState.TrackedTime.Value >= entitySpawnerSetting.SpawnPeriod.Value) 
                                {
                                    entitySpawnerState.TrackedTime.Value = 0;

                                    SpawnEntities(entitiesToSpawnList, entitySpawnerSetting, ref entitySpawnerState, translation);
                                }
                            }
                        }
                    }
                    else if((Time.ElapsedTime - entitySpawnerState.TrackedTime.Value) >= entitySpawnerSetting.SpawnPeriod.Value)
                    {
                        entitySpawnerState.TrackedTime.Value = Time.ElapsedTime;

                        SpawnEntities(entitiesToSpawnList, entitySpawnerSetting, ref entitySpawnerState, translation);
                    }
                }
            }
        }).Run();
    }

    private void SpawnEntities(NativeArray<EntitiesToSpawn> entitiesToSpawnList, in EntitySpawnerSetting entitySpawnerSetting, ref EntitySpawnerState entitySpawnerState, in FixTranslation translation)
    {
        var random = World.Random();

        for (int i = 0; i < entitySpawnerSetting.AmountSpawned; i++)
        {
            entitySpawnerState.IndexToSpawn = entitySpawnerSetting.SpawnedRandomly ? random.NextInt(0, entitiesToSpawnList.Length - 1) : entitySpawnerState.IndexToSpawn++;
            if (entitySpawnerState.IndexToSpawn >= entitiesToSpawnList.Length)
            {
                entitySpawnerState.IndexToSpawn = 0;
            }

            entitySpawnerState.TotalAmountSpawned++;

            Entity newEntity = EntityManager.Instantiate(entitiesToSpawnList[entitySpawnerState.IndexToSpawn]);

            if (EntityManager.TryGetComponentData(newEntity, out FixTranslation fixTranslation))
            {
                EntityManager.SetComponentData(newEntity, translation);
            }

            if (EntityManager.HasComponent<PhysicsVelocity>(newEntity))
            {
                fix2 impulseStrengh = new fix2() 
                { 
                    x = random.NextFix(entitySpawnerSetting.ShootDirectionMin.x, entitySpawnerSetting.ShootDirectionMax.x), 
                    y = random.NextFix(entitySpawnerSetting.ShootDirectionMin.y, entitySpawnerSetting.ShootDirectionMax.y)
                };

                impulseStrengh *= random.NextFix(entitySpawnerSetting.ShootSpeedMin, entitySpawnerSetting.ShootSpeedMax);

                CommonWrites.RequestImpulse(Accessor, newEntity, impulseStrengh);
            }
        }
    }
}