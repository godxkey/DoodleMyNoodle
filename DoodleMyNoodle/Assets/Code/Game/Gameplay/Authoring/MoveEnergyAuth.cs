using CCC.InspectorDisplay;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

//[DisallowMultipleComponent]
//public class MoveEnergyAuth : MonoBehaviour, IConvertGameObjectToEntity
//{
//    public int MaxValue = 10;
//    public bool StartAtMax = true;

//    [HideIf(nameof(StartAtMax))]
//    public int StartValue = 10;

//    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//    {
//        dstManager.AddComponentData(entity, new MoveEnergy { Value = StartAtMax ? MaxValue : StartValue });
//        dstManager.AddComponentData(entity, new MinimumFix<MoveEnergy> { Value = 0 });
//        dstManager.AddComponentData(entity, new MaximumFix<MoveEnergy> { Value = MaxValue });
//    }
//}
