using System;

/// <summary>
/// Blueprint ids are used to correctly recreate a gameobject at runtime from a saved game.
/// </summary>
[NetSerializable]
[Serializable]
public struct SimBlueprintId
{
    public enum BlueprintType : byte
    {
        Invalid,
        Prefab,
        SceneGameObject
    }

    public SimBlueprintId(BlueprintType type, string value)
    {
        this.Type = type;
        this.Value = value;
    }

    public BlueprintType Type;
    public string Value;

    public static readonly SimBlueprintId invalid = new SimBlueprintId(BlueprintType.Invalid, "");
    public bool IsValid => Value != invalid.Value;


    public override bool Equals(object obj)
    {
        return obj is SimBlueprintId id &&
               Type == id.Type &&
               Value == id.Value;
    }
    public static bool operator ==(SimBlueprintId a, SimBlueprintId b)
    {
        return a.Value == b.Value
            & a.Type == b.Type;
    }
    public static bool operator !=(SimBlueprintId a, SimBlueprintId b) => !(a == b);
    public override int GetHashCode() => Value.GetHashCode();
}
