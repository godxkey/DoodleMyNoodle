using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public struct NetMessageSimPlayerIdAssignement
{
    public PersistentId SimPlayerId;
    public PlayerId PlayerId;
}
