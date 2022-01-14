using CCC.Fix2D;
using Unity.Entities;

public struct NavAgentColliderRefs : IComponentData
{
    public BlobAssetReference<Collider> NormalCollider;
    public BlobAssetReference<Collider> AirControlCollider;
    public BlobAssetReference<Collider> DeadCollider;
}