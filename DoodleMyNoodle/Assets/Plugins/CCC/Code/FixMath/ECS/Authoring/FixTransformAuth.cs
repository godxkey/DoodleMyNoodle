using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class FixTransformAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [System.Serializable]
    public struct SerializedData
    {
        public FixVector3 LocalPosition;
        public FixQuaternion LocalRotation;
        public FixVector3 LocalScale;
        public FixTransformAuth Parent;
        public int SiblingIndex;
    }

    public FixVector3 LocalScale { get => _data.LocalScale; set { _data.LocalScale = value; } }
    public FixVector3 LocalPosition { get => _data.LocalPosition; set { _data.LocalPosition = value; } }
    public FixQuaternion LocalRotation { get => _data.LocalRotation; set { _data.LocalRotation = value; } }

    [UnityEngine.SerializeField]
    [CCC.InspectorDisplay.AlwaysExpand]
    public SerializedData _data = new SerializedData() // needs to be public for Editor access
    {
        LocalScale = new FixVector3(1, 1, 1)
    };

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new FixTranslation() { Value = LocalPosition });
        dstManager.AddComponentData(entity, new FixRotation() { Value = LocalRotation });
        dstManager.AddComponent<RemoveTransformInConversionTag>(entity);

        dstManager.RemoveComponent<Translation>(entity);
        dstManager.RemoveComponent<Rotation>(entity);
        // we don't do anything for the scale at the moment
    }
}

