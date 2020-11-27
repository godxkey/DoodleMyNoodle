using System.Collections;
using System.Collections.Generic;

[NetSerializable(IsBaseClass = true)]
public abstract class SimInput
{
}

/// <summary>
/// Inputs that only the master of the simulation can submit (used for validation)
/// </summary>
[NetSerializable(IsBaseClass = true)]
public abstract class SimMasterInput : SimInput
{
}

/// <summary>
/// Inputs meant for triggering cheats (removed from certain builds)
/// </summary>
[NetSerializable(IsBaseClass = true)]
public abstract class SimCheatInput : SimInput
{
}
