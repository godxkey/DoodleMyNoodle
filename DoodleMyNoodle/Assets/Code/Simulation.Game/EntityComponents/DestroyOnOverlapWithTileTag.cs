using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using System;

[GenerateAuthoringComponent]
public struct DestroyOnOverlapWithTileTag : IComponentData
{
    public bool DestroySelf;
    public bool DestroyTile;
}