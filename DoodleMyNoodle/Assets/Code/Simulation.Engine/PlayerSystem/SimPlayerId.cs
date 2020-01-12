using System;

[NetSerializable]
[System.Serializable]
public struct SimPlayerId : IDType
{
    public static readonly SimPlayerId Invalid = new SimPlayerId();
    public static readonly SimPlayerId FirstValid = new SimPlayerId(1);

    public SimPlayerId(SimPlayerId other) { Value = other.Value; }
    public SimPlayerId(UInt32 value) { this.Value = value; }

    public UInt32 Value;

    public bool IsValid => Value != Invalid.Value;



    #region Overloads
    public static SimPlayerId operator ++(SimPlayerId x) => new SimPlayerId((UInt16)(x.Value + 1));
    public static bool operator ==(SimPlayerId obj1, SimPlayerId obj2) => obj1.Value == obj2.Value;
    public static bool operator !=(SimPlayerId obj1, SimPlayerId obj2) => obj1.Value != obj2.Value;
    public override bool Equals(object obj)
    {
        if (!(obj is SimPlayerId))
        {
            return false;
        }

        var objPlayerId = (SimPlayerId)obj;
        return Value == objPlayerId.Value;
    }
    public override int GetHashCode()
    {
        return -1584136870 + Value.GetHashCode();
    }
    public override string ToString() => IsValid ? $"SimPlayerId({Value})" : "SimPlayerId(invalid)";
    #endregion

    public object GetValue()
    {
        return Value;
    }

    public void SetValue(object obj)
    {
        Value = Convert.ToUInt16(obj);
    }
}
