using System.Collections;
using System.Collections.Generic;

[NetSerializable(baseClass = true)]
public class SimPlayerInput : SimInput
{
    public SimPlayerId simPlayerId;

    public override void Execute(SimWorld world)
    {
    }
}
