

using System;

[NetSerializable]
[System.Serializable]
public partial struct PlayerId
{
    public static readonly PlayerId Invalid = new PlayerId();
    public static readonly PlayerId FirstValid = new PlayerId(1);

    public PlayerId(PlayerId other) { this.Value = other.Value; }
    public PlayerId(UInt16 value) { this.Value = value; }

    public UInt16 Value;

    public bool IsValid => this != Invalid;

    #region Overloads
    public static bool operator ==(PlayerId obj1, PlayerId obj2) => obj1.Value == obj2.Value;
    public static bool operator !=(PlayerId obj1, PlayerId obj2) => obj1.Value != obj2.Value;
    public override bool Equals(object obj)
    {
        if (!(obj is PlayerId))
        {
            return false;
        }

        var objPlayerId = (PlayerId)obj;
        return Value == objPlayerId.Value;
    }
    public override int GetHashCode()
    {
        return -1584136870 + Value.GetHashCode();
    }
    public override string ToString() => IsValid ? $"PlayerId({Value})" : "PlayerId(invalid)";
    #endregion
}