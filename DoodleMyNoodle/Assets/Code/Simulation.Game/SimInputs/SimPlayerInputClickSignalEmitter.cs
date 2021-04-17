using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[NetSerializable]
public class SimPlayerInputClickSignalEmitter : SimPlayerInput
{
    public fix2 EmitterPosition;

    public SimPlayerInputClickSignalEmitter() { }

    public SimPlayerInputClickSignalEmitter(fix2 position)
    {
        EmitterPosition = position;
    }

    public override string ToString()
    {
        return $"SimPlayerInputUseItem(player:{SimPlayerId.Value}, emitterPosition:{EmitterPosition})";
    }
}