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
        .ForEach((Entity entity, ref Dispenser dispenser, ref Duration duration, in DynamicBuffer<EntitiesToSpawn> entitiesToSpawn, in Signal signal, in FixTranslation translation) =>
        {
            if (dispenser.TotalAmountSpawned < dispenser.Quantity)
            {
                if (!dispenser.OnlyWhenSignalOn || signal.Value)
                {
                    NativeArray<EntitiesToSpawn> entitiesToSpawnList = entitiesToSpawn.ToNativeArray(Allocator.Temp);

                    if (!duration.IsSeconds)
                    {
                        if (HasSingleton<NewTurnEventData>())
                        {
                            if (duration.IsRounds && GetSingleton<TurnCurrentTeamSingletonComponent>().Value == 0)
                            {
                                duration.TrackingCount++;

                                if (duration.TrackingCount >= duration.Value)
                                {
                                    SpawnEntities(entitiesToSpawnList, ref dispenser, duration, translation);
                                }
                                
                            }
                            else if (duration.IsTurns)
                            {
                                duration.TrackingCount++;

                                if (duration.TrackingCount >= duration.Value) 
                                {
                                    SpawnEntities(entitiesToSpawnList, ref dispenser, duration, translation);
                                }
                            }
                        }
                    }
                    else if((Time.ElapsedTime - duration.LastTimeSpawned) >= duration.Value)
                    {
                        duration.LastTimeSpawned = Time.ElapsedTime;

                        SpawnEntities(entitiesToSpawnList, ref dispenser, duration, translation);
                    }
                }
            }
        }).Run();
    }

    private void SpawnEntities(NativeArray<EntitiesToSpawn> entitiesToSpawnList, ref Dispenser dispenser, in Duration duration, in FixTranslation translation)
    {
        var random = World.Random();

        for (int i = 0; i < dispenser.AmountSpawned; i++)
        {
            dispenser.IndexToSpawn = dispenser.SpawnedRandomly ? random.NextInt(0, entitiesToSpawnList.Length - 1) : dispenser.IndexToSpawn++;
            if (dispenser.IndexToSpawn >= entitiesToSpawnList.Length)
            {
                dispenser.IndexToSpawn = 0;
            }

            dispenser.TotalAmountSpawned++;

            Entity newEntity = EntityManager.Instantiate(entitiesToSpawnList[dispenser.IndexToSpawn]);

            if (EntityManager.TryGetComponentData(newEntity, out FixTranslation fixTranslation))
            {
                EntityManager.SetComponentData(newEntity, translation);
            }

            if (EntityManager.HasComponent<PhysicsVelocity>(newEntity))
            {
                fix2 impulseStrengh = new fix2() 
                { 
                    x = random.NextFix(dispenser.ShootDirectionMin.x, dispenser.ShootDirectionMax.x), 
                    y = random.NextFix(dispenser.ShootDirectionMin.y, dispenser.ShootDirectionMax.y)
                };

                impulseStrengh *= random.NextFix(dispenser.ShootSpeedMin, dispenser.ShootSpeedMax);

                CommonWrites.RequestImpulse(Accessor, newEntity, impulseStrengh);
            }
        }
    }
}