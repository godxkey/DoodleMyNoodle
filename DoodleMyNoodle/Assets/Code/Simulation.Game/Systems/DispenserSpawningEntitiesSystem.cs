using CCC.Fix2D;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class DispenserSpawningEntitiesSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        MultiTimeValue multiElapsedTime = GetElapsedTime();

        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((ref EntitySpawnerState state, in EntitySpawnerSetting settings, in DynamicBuffer<EntitiesToSpawn> entitiesToSpawn, in Signal signal, in FixTranslation position) =>
            {
                if (state.TotalAmountSpawned >= settings.Quantity)
                    return;

                if (settings.OnlyWhenSignalOn && !signal.Value)
                    return;

                TimeValue elapsedTime = multiElapsedTime.GetValue(settings.SpawnPeriod.Type);

                if ((elapsedTime - state.LastSpawnTime >= settings.SpawnPeriod)
                    || (settings.StartsReady && state.TotalAmountSpawned == 0))
                {
                    state.LastSpawnTime = elapsedTime;
                    SpawnEntities(entitiesToSpawn.ToNativeArray(Allocator.Temp), settings, ref state, position);
                }
            }).Run();
    }

    private void SpawnEntities(NativeArray<EntitiesToSpawn> entitiesToSpawnList, in EntitySpawnerSetting settings, ref EntitySpawnerState state, in FixTranslation position)
    {
        if (entitiesToSpawnList.Length == 0)
            return;

        var random = World.Random();

        for (int i = 0; i < settings.AmountSpawned; i++)
        {
            // if random, select a random index
            if (settings.SpawnedRandomly)
            {
                state.IndexToSpawn = random.NextInt(0, entitiesToSpawnList.Length - 1);
            }

            // make sure we don't overflow
            if (state.IndexToSpawn >= entitiesToSpawnList.Length)
            {
                state.IndexToSpawn = 0;
            }

            // track total entities spawned
            state.TotalAmountSpawned++;

            Entity newEntity = EntityManager.Instantiate(entitiesToSpawnList[state.IndexToSpawn]);

            // if not random, increment the index for the next time we spawn
            if (!settings.SpawnedRandomly)
            {
                state.IndexToSpawn++;
            }

            if (HasComponent<FixTranslation>(newEntity))
            {
                SetComponent(newEntity, position);
            }

            if (HasComponent<PhysicsVelocity>(newEntity))
            {
                fix2 startingVelocity = new fix2()
                {
                    x = random.NextFix(settings.ShootDirectionMin.x, settings.ShootDirectionMax.x),
                    y = random.NextFix(settings.ShootDirectionMin.y, settings.ShootDirectionMax.y)
                };

                startingVelocity *= random.NextFix(settings.ShootSpeedMin, settings.ShootSpeedMax);

                SetComponent(newEntity, new PhysicsVelocity()
                {
                    Linear = startingVelocity
                });
            }
        }
    }
}