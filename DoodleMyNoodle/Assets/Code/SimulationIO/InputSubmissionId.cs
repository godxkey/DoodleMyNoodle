
[NetSerializable]
public partial struct InputSubmissionId
{
    static byte _nextIdValue = 1;
    public static InputSubmissionId Generate() { return new InputSubmissionId(_nextIdValue++); }

    public InputSubmissionId(InputSubmissionId other) { this.value = other.value; }
    public InputSubmissionId(byte value) { this.value = value; }

    public byte value;

    public static readonly InputSubmissionId Invalid = default;

    #region Overloads
    public static bool operator ==(InputSubmissionId obj1, InputSubmissionId obj2) => obj1.value == obj2.value;
    public static bool operator !=(InputSubmissionId obj1, InputSubmissionId obj2) => obj1.value != obj2.value;
    public override bool Equals(object obj)
    {
        if (!(obj is InputSubmissionId))
        {
            return false;
        }

        var objInputSubissionId = (InputSubmissionId)obj;
        return value == objInputSubissionId.value;
    }
    public override int GetHashCode()
    {
        return -1584136870 + value.GetHashCode();
    }
    #endregion
}