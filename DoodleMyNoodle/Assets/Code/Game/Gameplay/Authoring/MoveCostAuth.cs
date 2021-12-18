using CCC.InspectorDisplay;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class MoveCostAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public float StartValue = 1;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MoveCost { Value = (fix)StartValue });
    }
}
