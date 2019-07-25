using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public class SimCommandLog : SimCommand
{
    public string message;

    public override void Execute()
    {
        DebugService.Log(message);
    }
}
