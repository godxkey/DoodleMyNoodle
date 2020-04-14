using System;
using Unity.Entities;

[Serializable]
public struct SimAssetId : IComponentData, IEquatable<SimAssetId>
{
    public int Value;

    public static SimAssetId Null => new SimAssetId();

    public override bool Equals(object obj) => Equals((SimAssetId)obj);
    public override int GetHashCode() => Value.GetHashCode();
    public bool Equals(SimAssetId other) => other.Value == Value;
    public static bool operator ==(SimAssetId a, SimAssetId b) => a.Equals(b);
    public static bool operator !=(SimAssetId a, SimAssetId b) => !(a.Value == b.Value);
}
