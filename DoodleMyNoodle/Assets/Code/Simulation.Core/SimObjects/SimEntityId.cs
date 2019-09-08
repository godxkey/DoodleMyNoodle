using System;

[NetSerializable]
[Serializable]
public struct SimEntityId
{
    public static readonly SimEntityId Invalid = new SimEntityId();
    public static readonly SimEntityId FirstValid = new SimEntityId(1);

    public SimEntityId(SimEntityId other) { Value = other.Value; }
    public SimEntityId(UInt16 value) { this.Value = value; }

    public UInt16 Value;

    public bool IsValid => this != Invalid;

    #region Overloads
    public static SimEntityId operator ++(SimEntityId x) => new SimEntityId((UInt16)(x.Value + 1));
    public static bool operator ==(SimEntityId obj1, SimEntityId obj2) => obj1.Value == obj2.Value;
    public static bool operator !=(SimEntityId obj1, SimEntityId obj2) => obj1.Value != obj2.Value;
    public override bool Equals(object obj)
    {
        if (!(obj is SimEntityId))
        {
            return false;
        }

        var objPlayerId = (SimEntityId)obj;
        return Value == objPlayerId.Value;
    }

    public override int GetHashCode()
    {
        return -1584136870 + Value.GetHashCode();
    }
    #endregion
}
