
// fbessette: je ne pense pas qu'on aura plus qu'1 type ou 2 de Blueprint

/// <summary>
/// Base class for our blueprint types
/// </summary>
[System.Serializable]
public class SimBlueprint
{
    public SimBlueprint(SimBlueprintId id, SimEntity prefab)
    {
        this.id = id;
        this.prefab = prefab;
    }

    public readonly SimBlueprintId id;
    public readonly SimEntity prefab;
}