
// fbessette: je ne pense pas qu'on aura plus qu'1 type ou 2 de Blueprint

[System.Serializable]
public struct SimBlueprint
{
    public SimBlueprint(SimBlueprintId id, SimEntity prefab)
    {
        this.Id = id;
        this.Prefab = prefab;
    }

    public bool IsValid => Prefab != null;

    public readonly SimBlueprintId Id;
    public readonly SimEntity Prefab;
}