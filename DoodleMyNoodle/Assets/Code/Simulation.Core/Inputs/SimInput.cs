using System.Collections;
using System.Collections.Generic;

[NetSerializable(baseClass = true)]
public abstract class SimInput
{
    public abstract void Execute(SimWorld world);
}
