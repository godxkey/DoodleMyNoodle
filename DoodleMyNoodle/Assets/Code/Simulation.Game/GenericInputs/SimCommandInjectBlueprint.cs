using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CETTE INPUT EST TEMPORAIRE ET NE DEVRAIS LOGIQUEMENT PAS ÊTRE UTILISÉ DANS LE JEU FINALE
/// <para/>
/// (world.Instantiate(blueprint) devrait être appelé de l'intérieur de la simulation)
/// </summary>
[NetSerializable]
public class SimCommandInjectBlueprint : SimCommand
{
    public SimBlueprintId blueprintId;

    public override void Execute()
    {
        SimBlueprint blueprint = Simulation.GetBlueprint(blueprintId);

        if (blueprint.IsValid)
        {
            Simulation.Instantiate(blueprint);
        }
        else
        {
            DebugService.LogError($"Could not inject blueprint [id:{blueprintId}]. It was not retreived in the blueprint bank");
        }
    }
}
