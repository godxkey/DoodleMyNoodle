using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public class SimCommandInjectBlueprint : SimCommand
{
    public SimBlueprintId blueprintId;

    public override void Execute(SimWorld world)
    {
        SimBlueprint blueprint = Simulation.blueprintBank.GetBlueprint(blueprintId);

        if (blueprint != null)
        {
            world.Instantiate(blueprint);
        }
        else
        {
            DebugService.LogError($"Could not inject blueprint [id:{blueprintId}]. It was not retreived in the blueprint bank");
        }
    }
}
