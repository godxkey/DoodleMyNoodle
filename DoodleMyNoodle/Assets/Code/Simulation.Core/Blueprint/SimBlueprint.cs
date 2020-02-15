
// fbessette: cette struct pourrait probablement être retiré. Elle ne semble pas nécessaire.

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