using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PawnControllerInputClickSignalEmitter : PawnControllerInputBase
{
    public Entity Emitter;

    public PawnControllerInputClickSignalEmitter(Entity pawnController, Entity emitter) : base(pawnController)
    {
        Emitter = emitter;
    }

    public override string ToString()
    {
        return $"PawnControllerInputUseItem(pawnControlled: {PawnController}, emitterPosition: {Emitter})";
    }
}