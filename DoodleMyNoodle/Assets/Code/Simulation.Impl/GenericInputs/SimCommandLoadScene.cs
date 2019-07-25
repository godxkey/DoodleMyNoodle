using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CETTE INPUT EST TEMPORAIRE ET NE DEVRAIS LOGIQUEMENT PAS ÊTRE UTILISÉ DANS LE JEU FINALE
/// <para/>
/// (world.InjectScene(sceneName) devrait être appelé de l'intérieur de la simulation)
/// </summary>
[NetSerializable]
public class SimCommandLoadScene : SimCommand
{
    public string sceneName;

    public override void Execute()
    {
        Simulation.LoadScene(sceneName);
    }
}
