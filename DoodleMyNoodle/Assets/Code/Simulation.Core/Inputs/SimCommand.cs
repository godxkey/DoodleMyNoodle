using System.Collections;
using System.Collections.Generic;

[NetSerializable(baseClass = true)]
public abstract class SimCommand : SimInput
{
    public abstract void Execute();
}
