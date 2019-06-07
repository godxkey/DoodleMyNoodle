using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public class SimInputInstantiate : SimInput
{
    public SimBlueprintId blueprintId;

    public override void Execute(SimWorld world)
    {
        world.InstantiateEntity(blueprintId);
    }
}
