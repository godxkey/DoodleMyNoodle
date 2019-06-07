
// fbessette: je ne pense pas qu'on aura plus qu'1 type ou 2 de Blueprint

/// <summary>
/// Base class for our blueprint types
/// </summary>
[System.Serializable]
public abstract class SimBlueprint
{
    public abstract void InstantiateEntityAndView(out SimEntity entity, out SimEntityView view);
}