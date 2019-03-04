using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetMessageFactory
{
    ushort GetNetMessageTypeId(NetMessage message);
    NetMessage CreateNetMessage(ushort messageType);
}
