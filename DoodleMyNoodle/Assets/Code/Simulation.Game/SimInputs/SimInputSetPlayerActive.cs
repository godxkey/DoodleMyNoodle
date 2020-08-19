using System;
using UnityEngine;
using UnityEngineX;

[NetSerializable]
public class SimInputSetPlayerActive : SimMasterInput
{
    public PersistentId PlayerID;
    public bool IsActive;
}