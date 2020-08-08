using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[NetSerializable]
public class SimPlayerInputSetPawnName : SimPlayerInput
{
    public string Name;

    public SimPlayerInputSetPawnName() : base() { }

    public SimPlayerInputSetPawnName(string name) : base()
    {
        Name = name;
    }
}

public class PawnControllerInputSetPawnName : PawnControllerInputBase
{
    public string Name;

    public PawnControllerInputSetPawnName(Entity pawnController) : base(pawnController) { }

    public PawnControllerInputSetPawnName(Entity pawnController, string name) : base(pawnController)
    {
        Name = name;
    }
}