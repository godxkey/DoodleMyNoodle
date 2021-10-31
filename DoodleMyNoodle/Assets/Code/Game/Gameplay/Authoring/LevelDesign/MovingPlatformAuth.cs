using CCC.Fix2D.Authoring;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(PhysicsBodyAuth))]
[ExecuteInEditMode]
public class MovingPlatformAuth : MonoBehaviour, IConvertGameObjectToEntity, SignalAuth.Gizmos.ISignalListener
{
    [Serializable]
    public class Point
    {
        public Vector2 Position;
        [FormerlySerializedAs("SignalEmitter")]
        public SignalAuth ConditionalEmitter;
    }

    public Point[] MovePoints;
    public float MaximumSpeed = 3f;
    [FormerlySerializedAs("Move")]
    public PlatformMoveMode MoveMode = PlatformMoveMode.Yoyo;
    public bool SlowDownNearPoints = true;
    public bool PauseOnPoints = true;
    public TimeValue PauseDuration = TimeValue.Seconds(1);

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MoveSpeed() { Value = (fix)MaximumSpeed });
        dstManager.AddComponentData(entity, new MovingPlatformSettings()
        {
            MoveMode = MoveMode,
            PauseOnNodesDuration = PauseOnPoints ? PauseDuration : TimeValue.Zero,
            SlowDownNearNodes = SlowDownNearPoints
        });
        dstManager.AddComponentData(entity, new MovingPlatformState() { });

        DynamicBuffer<PathPosition> points = dstManager.AddBuffer<PathPosition>(entity);
        Vector2 positionOffset = transform.position;

        if (MoveMode == PlatformMoveMode.Signals)
        {
            var signalPositions = dstManager.AddBuffer<MovingPlatformSignalPosition>(entity);
            signalPositions.Capacity = MovePoints.Length;

            foreach (var item in MovePoints)
            {
                signalPositions.Add(new MovingPlatformSignalPosition()
                {
                    Position = (fix2)(positionOffset + item.Position),
                    ConditionalEmitter = item.ConditionalEmitter == null ? Entity.Null : conversionSystem.GetPrimaryEntity(item.ConditionalEmitter)
                });
            }
        }
        else
        {
            points.Capacity = MovePoints.Length;
            foreach (var item in MovePoints)
            {
                points.Add((fix2)(positionOffset + item.Position));
            }
        }
    }

    private void OnEnable()
    {
        SignalAuth.Gizmos.Register(this);
    }

    private void OnDisable()
    {
        SignalAuth.Gizmos.Unregister(this);
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 positionOffset = transform.position;
        Gizmos.color = Color.magenta;
        for (int i = 1; i < MovePoints.Length; i++)
        {
            Gizmos.DrawLine(MovePoints[i - 1].Position + positionOffset, MovePoints[i].Position + positionOffset);
        }

        if (MoveMode == PlatformMoveMode.Signals)
        {
            SignalAuth.Gizmos.DrawSignalGizmos(this);
        }
    }

    void SignalAuth.Gizmos.ISignalListener.GetSignalReferences(List<SignalAuth> result)
    {
        if (MoveMode == PlatformMoveMode.Signals)
        {
            foreach (var point in MovePoints)
            {
                if (point.ConditionalEmitter != null)
                    result.Add(point.ConditionalEmitter);
            }
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