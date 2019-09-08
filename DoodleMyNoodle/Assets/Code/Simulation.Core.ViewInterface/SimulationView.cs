using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationView : SimulationBase
{
    public static void Initialize(ISimModuleBlueprintBank blueprintBank) => SimModules.Initialize(blueprintBank);
    public static void Dispose() => SimModules.Dispose();

    /// <summary>
    /// Set the next time id the sim will execute
    /// </summary>
    public static void ForceSetTickId(uint tickId) => SimModules.World.TickId = tickId;
}
