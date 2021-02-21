using CCC.Fix2D;
using CCC.Fix2D.Authoring;
using CCC.InspectorDisplay;
using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;
using Collider = CCC.Fix2D.Collider;

[RequiresEntityConversion]
[RequireComponent(typeof(PhysicsBodyAuth))]
public class MovingPlatformAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public Vector2[] MovePoints;
    public float MaximumSpeed = 3f;
    public PlatformMoveMode Move = PlatformMoveMode.Yoyo;
    public bool SlowDownNearPoints = true;
    public bool PauseOnPoints = true;
    [ShowIf(nameof(PauseOnPoints))]
    public float PauseDuration = 1f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MoveSpeed() { Value = (fix)MaximumSpeed });
        dstManager.AddComponentData(entity, new MovingPlatformSettings()
        {
            MoveMode = Move,
            PauseOnNodesDuration = PauseOnPoints ? (fix)PauseDuration : 0,
            SlowDownNearNodes = SlowDownNearPoints
        });
        dstManager.AddComponentData(entity, new MovingPlatformState() { });

        DynamicBuffer<PathPosition> points = dstManager.AddBuffer<PathPosition>(entity);
        points.Capacity = MovePoints.Length;
        foreach (Vector2 item in MovePoints)
        {
            points.Add((fix2)item);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        for (int i = 1; i < MovePoints.Length; i++)
        {
            Gizmos.DrawLine(MovePoints[i - 1], MovePoints[i]);
        }
    }
}

//[UpdateInGroup(typeof(GameObjectConversionGroup))]
//public class MovingPlatformConversionSystem : GameObjectConversionSystem
//{
//    private ConvertToFixTransformSystem _convertToFixTransform;

//    protected override void OnCreate()
//    {
//        base.OnCreate();

//        _convertToFixTransform = World.GetOrCreateSystem<ConvertToFixTransformSystem>();
//    }

//    protected override void OnUpdate()
//    {
//        Entities.ForEach((MovingPlatformAuth movingPlatform) =>
//        {
//            Entity entity = GetPrimaryEntity(movingPlatform);
//            BoxGeometry boxGeometry = new BoxGeometry() { Size = movingPlatform.Size };
//            BlobAssetReference<Collider> collider = Collider.Create(boxGeometry, CollisionFilter.FromLayer(movingPlatform.gameObject.layer), movingPlatform.Material);

//            DstEntityManager.AddComponentData(entity, new PhysicsColliderBlob { Collider = collider });
//            DstEntityManager.SetOrAddComponentData(entity, PhysicsVelocity.Zero);
//            DstEntityManager.SetOrAddComponentData(entity, PhysicsMass.CreateKinematic(MassProperties.Default));

//            _convertToFixTransform.ToConvert.Add(movingPlatform.transform);
//        });
//    }
//}