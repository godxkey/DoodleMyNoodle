using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public struct NetMessageSimPlayerIdAssignement
{
    public SimPlayerId SimPlayerId;
    public PlayerId PlayerId;
}
