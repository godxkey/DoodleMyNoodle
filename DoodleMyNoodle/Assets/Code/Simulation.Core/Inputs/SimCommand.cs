using System.Collections;
using System.Collections.Generic;

[NetSerializable(IsBaseClass = true)]
public abstract class SimCommand : SimInput
{
    public abstract void Execute();
}
