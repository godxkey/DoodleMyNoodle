using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public class SimCommandLoadScene : SimMasterInput
{
    public string SceneName;

    public override string ToString()
    {
        return $"SimCommandLoadScene({SceneName})";
    }
}
