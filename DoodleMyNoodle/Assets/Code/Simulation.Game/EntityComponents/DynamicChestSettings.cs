using Unity.Entities;


/// <summary>
/// This component is placed on the chest formula entity
/// </summary>
public struct DynamicChestFormulaSettings : IComponentData
{
    public fix BudgetBase;
    public Unity.Animation.AnimationCurve TeamHealthRatioBudget;
    public Unity.Animation.AnimationCurve ConsumablesPerPawnBudget;
    public fix BudgetMin;
    public fix BudgetMax;
}

/// <summary>
/// This buffer element type is placed on the chest formula entity
/// </summary>
public struct DynamicChestFormulaItemEntry : IBufferElementData
{
    public Entity Item;
    public fix Weight;
    public fix BudgetCost;
}

/// <summary>
/// This component is placed on dynamic chests
/// </summary>
public struct DynamicChestFormulaRef : IComponentData
{
    public Entity FormulaEntity;
}

/// <summary>
/// This component is placed on dynamic chests
/// </summary>
public struct DynamicChestFillOnNextUpdateToken : IComponentData
{
    public bool Value;

    public static implicit operator bool(DynamicChestFillOnNextUpdateToken val) => val.Value;
    public static implicit operator DynamicChestFillOnNextUpdateToken(bool val) => new DynamicChestFillOnNextUpdateToken() { Value = val };
}