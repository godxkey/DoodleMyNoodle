using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[NetSerializable]
public class SimPlayerInputClickSignalEmitter : SimPlayerInput
{
    public Entity Emitter;

    public SimPlayerInputClickSignalEmitter() { }

    public SimPlayerInputClickSignalEmitter(Entity emitter)
    {
        Emitter = emitter;
    }

    public override string ToString()
    {
        return $"SimPlayerInputUseItem(player:{SimPlayerId.Value}, emitter:{Emitter})";
    }
}