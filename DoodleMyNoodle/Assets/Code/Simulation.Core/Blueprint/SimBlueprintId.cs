using System;

[NetSerializable]
[Serializable]
public struct SimBlueprintId
{
    public enum Type : byte
    {
        Invalid,
        Prefab,
        SceneGameObject
    }

    public SimBlueprintId(Type type, string value)
    {
        this.type = type;
        this.value = value;
    }

    public Type type;
    public string value;

    public static readonly SimBlueprintId invalid = new SimBlueprintId(Type.Invalid, "");
    public bool isValid => value != invalid.value;


    public override bool Equals(object obj)
    {
        return obj is SimBlueprintId id &&
               type == id.type &&
               value == id.value;
    }
    public static bool operator ==(SimBlueprintId a, SimBlueprintId b)
    {
        return a.value == b.value
            & a.type == b.type;
    }
    public static bool operator !=(SimBlueprintId a, SimBlueprintId b) => !(a == b);
    public override int GetHashCode() => value.GetHashCode();
}
