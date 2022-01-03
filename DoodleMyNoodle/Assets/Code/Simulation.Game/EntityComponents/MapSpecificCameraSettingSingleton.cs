using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

[GenerateAuthoringComponent]
public struct MapSpecificCameraSettingSingleton : IComponentData
{
    public fix2 Position;
    public fix Zoom;
}