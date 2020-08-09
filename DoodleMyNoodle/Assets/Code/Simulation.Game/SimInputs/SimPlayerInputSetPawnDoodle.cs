using System;
using UnityEngine;
using UnityEngineX;

[NetSerializable]
public class SimPlayerInputSetPawnDoodle : SimPlayerInput
{
    public Guid DoodleId;

    public SimPlayerInputSetPawnDoodle() { } // needed for net serializer

    public SimPlayerInputSetPawnDoodle(Guid doodleId)
    {
        DoodleId = doodleId;
    }
}