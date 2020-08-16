using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public struct PhysicsGravity : IComponentData
{
    public fix Scale;
}