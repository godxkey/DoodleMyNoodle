using System.Collections;
using System.Collections.Generic;

[NetSerializable(baseClass = true)]
public abstract class SimInput
{
}

/// <summary>
/// Inputs that only the master of the simulation can submit (used for validation)
/// </summary>
[NetSerializable(baseClass = true)]
public abstract class SimMasterInput : SimInput
{
}
