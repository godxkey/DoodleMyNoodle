using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class DispenserAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    private enum DurationType { Seconds, Turns, Rounds }

    [SerializeField]
    private DurationType DelayType;

    [SerializeField]
    private float Delay;

    [SerializeField]
    private bool SpawnOnlyWhenSignalOn = true;
    [SerializeField]
    private int QuantitySpawnedEachTick = 1;

    [SerializeField]
    private bool IsRandomAmongList = false;

    [SerializeField]
    private float ShootSpeedMinimum = 1;
    [SerializeField]
    private float ShootSpeedMaximum = 2;

    [SerializeField]
    private Vector2 ShootDirectionMinimum;
    [SerializeField]
    private Vector2 ShootDirectionMaximum;

    [SerializeField]
    private int TotalQuantityAvailable = 1;

    [SerializeField]
    private List<GameObject> EntitiesToSpawn;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Duration() 
        { 
            Value = (fix)Delay, 
            IsRounds = DelayType == DurationType.Rounds,
            IsSeconds = DelayType == DurationType.Seconds,
            IsTurns = DelayType == DurationType.Turns,
        });

        dstManager.AddComponentData(entity, new Dispenser()
        {
            OnlyWhenSignalOn = SpawnOnlyWhenSignalOn,
            ShootSpeedMin = (fix)ShootSpeedMinimum,
            ShootSpeedMax = (fix)ShootSpeedMaximum,
            ShootDirectionMin = new fix2() { x = (fix)ShootDirectionMinimum.x, y = (fix)ShootDirectionMinimum.y },
            ShootDirectionMax = new fix2() { x = (fix)ShootDirectionMaximum.x, y = (fix)ShootDirectionMaximum.y },
            AmountSpawned = QuantitySpawnedEachTick,
            Quantity = TotalQuantityAvailable,
            SpawnedRandomly = IsRandomAmongList,
            IndexToSpawn = 0
        });

        var entities = dstManager.AddBuffer<EntitiesToSpawn>(entity);

        foreach (var gameObject in EntitiesToSpawn)
        {
            if (gameObject != null)
                entities.Add(conversionSystem.GetPrimaryEntity(gameObject));
        }
    }
}