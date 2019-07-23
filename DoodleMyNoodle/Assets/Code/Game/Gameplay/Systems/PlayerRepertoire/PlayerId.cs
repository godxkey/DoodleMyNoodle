

using System;

[NetSerializable]
public partial struct PlayerId
{
    public static readonly PlayerId invalid = new PlayerId();
    public static readonly PlayerId firstValid = new PlayerId(1);

    public PlayerId(PlayerId other) { this.value = other.value; }
    public PlayerId(UInt16 value) { this.value = value; }

    public UInt16 value;

    public bool isValid => this != invalid;

    #region Overloads
    public static bool operator ==(PlayerId obj1, PlayerId obj2) => obj1.value == obj2.value;
    public static bool operator !=(PlayerId obj1, PlayerId obj2) => obj1.value != obj2.value;
    public override bool Equals(object obj)
    {
        if (!(obj is PlayerId))
        {
            return false;
        }

        var objPlayerId = (PlayerId)obj;
        return value == objPlayerId.value;
    }
    public override int GetHashCode()
    {
        return -1584136870 + value.GetHashCode();
    }
    #endregion
}