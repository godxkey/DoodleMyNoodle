using System;
using Unity.Entities;

[Serializable]
public struct SimAssetId : IComponentData, IEquatable<SimAssetId>
{
    public ushort Value;

    public static SimAssetId Invalid => new SimAssetId();

    public SimAssetId(ushort value)
    {
        Value = value;
    }

    public override bool Equals(object obj) => Equals((SimAssetId)obj);
    public override int GetHashCode() => Value.GetHashCode();
    public bool Equals(SimAssetId other) => other.Value == Value;
    public static bool operator ==(SimAssetId a, SimAssetId b) => a.Equals(b);
    public static bool operator !=(SimAssetId a, SimAssetId b) => !(a.Value == b.Value);
}
