
using UnityEngine;

[NetSerializable]
public partial struct InputSubmissionId
{
    static byte s_nextIdValue = 1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void StaticReset()
    {
        s_nextIdValue = 1;
    }

    public static InputSubmissionId Generate() { return new InputSubmissionId(s_nextIdValue++); }

    public InputSubmissionId(InputSubmissionId other) { this.Value = other.Value; }
    public InputSubmissionId(byte value) { this.Value = value; }

    public byte Value;

    public static readonly InputSubmissionId Invalid = default;

    #region Overloads
    public static bool operator ==(InputSubmissionId obj1, InputSubmissionId obj2) => obj1.Value == obj2.Value;
    public static bool operator !=(InputSubmissionId obj1, InputSubmissionId obj2) => obj1.Value != obj2.Value;
    public override bool Equals(object obj)
    {
        if (!(obj is InputSubmissionId))
        {
            return false;
        }

        var objInputSubissionId = (InputSubmissionId)obj;
        return Value == objInputSubissionId.Value;
    }
    public override int GetHashCode()
    {
        return -1584136870 + Value.GetHashCode();
    }
    #endregion
}