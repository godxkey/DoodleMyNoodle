using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using System;

[Serializable]
public struct WinningTeam : IComponentData
{
    public int Value;
}
