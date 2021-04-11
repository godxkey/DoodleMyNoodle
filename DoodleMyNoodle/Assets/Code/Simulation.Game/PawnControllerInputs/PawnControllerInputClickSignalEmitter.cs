using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PawnControllerInputClickSignalEmitter : PawnControllerInputBase
{
    public fix2 EmitterPosition;

    public PawnControllerInputClickSignalEmitter(Entity pawnController, fix2 emitterPosition) : base(pawnController)
    {
        EmitterPosition = emitterPosition;
    }

    public override string ToString()
    {
        return $"PawnControllerInputUseItem(pawnControlled: {PawnController}, emitterPosition: {EmitterPosition})";
    }
}