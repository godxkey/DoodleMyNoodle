using System;

[NetSerializable]
[Serializable]
public struct SimObjectId : IDType
{
    public static readonly SimObjectId Invalid = new SimObjectId();
    public static readonly SimObjectId FirstValid = new SimObjectId(1);

    public SimObjectId(SimObjectId other) { Value = other.Value; }
    public SimObjectId(UInt32 value) { this.Value = value; }

    public UInt32 Value;

    public bool IsValid => this != Invalid;



    #region Overloads
    public static SimObjectId operator ++(SimObjectId x) => new SimObjectId((UInt32)(x.Value + 1));
    public static bool operator ==(SimObjectId obj1, SimObjectId obj2) => obj1.Value == obj2.Value;
    public static bool operator !=(SimObjectId obj1, SimObjectId obj2) => obj1.Value != obj2.Value;
    public override bool Equals(object obj)
    {
        if (!(obj is SimObjectId))
        {
            return false;
        }

        var objPlayerId = (SimObjectId)obj;
        return Value == objPlayerId.Value;
    }

    public override int GetHashCode()
    {
        return -1584136870 + Value.GetHashCode();
    }
    public override string ToString() => IsValid ? $"SimObjectId({Value})" : "SimObjectId(invalid)";

    #endregion

    public object GetValue()
    {
        return Value;
    }

    public void SetValue(object obj)
    {
        Value = Convert.ToUInt32(obj);
    }
}
