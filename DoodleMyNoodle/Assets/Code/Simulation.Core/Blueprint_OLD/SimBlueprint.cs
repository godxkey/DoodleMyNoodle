using System.Diagnostics;

// fbessette: cette struct pourrait probablement être retiré. Elle ne semble pas nécessaire.


[System.Serializable]
[DebuggerDisplay("Prefab: {Prefab}  Id: {Id}")]
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