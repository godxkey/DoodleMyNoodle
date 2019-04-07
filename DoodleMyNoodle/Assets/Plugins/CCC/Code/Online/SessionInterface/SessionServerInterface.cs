using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionServerInterface : SessionInterface
{
    public SessionServerInterface(NetworkInterface networkInterface) : base(networkInterface) { }

    public override bool isServerType => true;
}
