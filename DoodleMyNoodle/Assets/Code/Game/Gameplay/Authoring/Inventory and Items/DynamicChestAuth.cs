using CCC.Fix2D;
using System.Collections.Generic;
using Unity.Animation;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(InventoryAuth))]
[RequireComponent(typeof(InteractableAuth))]
public class DynamicChestAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField] private DynamicChestFormulaAuth _formula = null;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (_formula != null)
        {
            dstManager.AddComponentData(entity, new DynamicChestFormulaRef() { FormulaEntity = conversionSystem.GetPrimaryEntity(_formula) });
            dstManager.AddComponent<DynamicChestFillOnNextUpdateToken>(entity);
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        if (_formula != null)
            referencedPrefabs.Add(_formula.gameObject);
    }
}