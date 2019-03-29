using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetMessageFactory
{
    ushort GetNetMessageTypeId(INetSerializable message);
    INetSerializable CreateNetMessage(ushort messageType);
}
