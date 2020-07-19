using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[NetSerializable]
public class SimPlayerCharacterNameInput : SimPlayerInput
{
    public string Name;

    public SimPlayerCharacterNameInput() : base() { }

    public SimPlayerCharacterNameInput(string name) : base()
    {
        Name = name;
    }
}

public class PawnCharacterNameInput : PawnControllerInputBase
{
    public string Name;

    public PawnCharacterNameInput(Entity pawnController) : base(pawnController) { }

    public PawnCharacterNameInput(Entity pawnController, string name) : base(pawnController)
    {
        Name = name;
    }
}