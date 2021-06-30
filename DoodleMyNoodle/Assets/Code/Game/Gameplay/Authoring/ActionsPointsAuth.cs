using CCC.InspectorDisplay;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class ActionsPointsAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int MaxValue = 4;
    public bool StartGameAtMaxValue = true;

    [HideIf(nameof(StartGameAtMaxValue))]
    public int StartValue = 4;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (StartGameAtMaxValue)
        {
            StartValue = MaxValue;
        }

        dstManager.AddComponentData(entity, new ActionPoints { Value = StartValue });
        dstManager.AddComponentData(entity, new MinimumInt<ActionPoints> { Value = 0 });
        dstManager.AddComponentData(entity, new MaximumInt<ActionPoints> { Value = MaxValue });
    }
}
