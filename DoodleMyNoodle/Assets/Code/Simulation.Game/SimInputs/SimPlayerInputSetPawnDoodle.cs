using System;
using UnityEngine;
using UnityEngineX;

[NetSerializable]
public class SimPlayerInputSetPawnDoodle : SimPlayerInput
{
    public Guid DoodleId;
    public bool DoodleDirectionIsLookingRight;

    public SimPlayerInputSetPawnDoodle() { } // needed for net serializer

    public SimPlayerInputSetPawnDoodle(Guid doodleId, bool IsLookingRight)
    {
        DoodleId = doodleId;
        DoodleDirectionIsLookingRight = IsLookingRight;
    }
}