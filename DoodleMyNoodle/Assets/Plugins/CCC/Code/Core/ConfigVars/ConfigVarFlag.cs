using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum ConfigVarFlag
{
    None = 0,
    
    // Causes the cvar to be save to settings.cfg
    Save = 1 << 0,
}
