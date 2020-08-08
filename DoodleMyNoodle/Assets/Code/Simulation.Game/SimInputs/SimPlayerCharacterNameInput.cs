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

public class PawnControllerInputCharacterName : PawnControllerInputBase
{
    public string Name;

    public PawnControllerInputCharacterName(Entity pawnController) : base(pawnController) { }

    public PawnControllerInputCharacterName(Entity pawnController, string name) : base(pawnController)
    {
        Name = name;
    }
}