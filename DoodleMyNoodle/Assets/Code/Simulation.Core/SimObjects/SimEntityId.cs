using System;

[NetSerializable]
[Serializable]
public struct SimEntityId
{
    public static readonly SimEntityId invalid = new SimEntityId();
    public static readonly SimEntityId firstValid = new SimEntityId(1);

    public SimEntityId(SimEntityId other) { value = other.value; }
    public SimEntityId(UInt16 value) { this.value = value; }

    public UInt16 value;

    public void Increment()
    {
        value++;
    }

    public bool isValid => this != invalid;

    #region Overloads
    public static bool operator ==(SimEntityId obj1, SimEntityId obj2) => obj1.value == obj2.value;
    public static bool operator !=(SimEntityId obj1, SimEntityId obj2) => obj1.value != obj2.value;
    public override bool Equals(object obj)
    {
        if (!(obj is SimEntityId))
        {
            return false;
        }

        var objPlayerId = (SimEntityId)obj;
        return value == objPlayerId.value;
    }

    public override int GetHashCode()
    {
        return -1584136870 + value.GetHashCode();
    }
    #endregion
}
