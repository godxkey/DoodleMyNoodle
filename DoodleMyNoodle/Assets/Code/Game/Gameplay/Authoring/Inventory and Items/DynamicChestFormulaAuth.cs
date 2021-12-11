using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Animation.Hybrid;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class DynamicChestFormulaAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [System.Serializable]
    private class ItemEntry
    {
        public ItemAuth Item = null;
        public float Weight = 1f;
        public float BudgetCost = 1f;
    }

    [SerializeField] private List<ItemEntry> _items = new List<ItemEntry>();

    [Header("Budget Sources")]
    [FormerlySerializedAs("_baseBudget")]
    [SerializeField] private float _baseBudget = 0.25f;
    [Header("Budget Multipliers")]
    [FormerlySerializedAs("_teamHealthRatioBudget")]
    [SerializeField] private AnimationCurve _teamHealthRatio = new AnimationCurve();
    [FormerlySerializedAs("_consumablesPerPawnBudget")]
    [SerializeField] private AnimationCurve _consumablesPerPawn = new AnimationCurve();

    [Header("Budget Clamping")]
    [SerializeField] private float _minimumBudget = 1f;
    [SerializeField] private float _maximumBudget = 10f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new DynamicChestFormulaSettings()
        {
            BudgetBase = (fix)_baseBudget,
            BudgetMin = (fix)_minimumBudget,
            BudgetMax = (fix)_maximumBudget,
            TeamHealthRatioBudget = _teamHealthRatio.ToDotsAnimationCurve(),
            ConsumablesPerPawnBudget = _consumablesPerPawn.ToDotsAnimationCurve(),
        });

        var itemsBuffer = dstManager.AddBuffer<DynamicChestFormulaItemEntry>(entity);
        foreach (var item in _items)
        {
            if (item.Weight > 0 && item.Item != null)
            {
                itemsBuffer.Add(new DynamicChestFormulaItemEntry()
                {
                    BudgetCost = (fix)item.BudgetCost,
                    Weight = (fix)item.Weight,
                    Item = conversionSystem.GetPrimaryEntity(item.Item),
                });
            }
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (var item in _items)
        {
            if (item.Weight > 0 && item.Item != null)
            {
                referencedPrefabs.Add(item.Item.gameObject);
            }
        }
    }
}