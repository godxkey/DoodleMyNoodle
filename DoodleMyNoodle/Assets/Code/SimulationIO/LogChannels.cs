using System;
using UnityEngine;
using UnityEngineX;

#pragma warning disable IDE1006 // Naming Styles
internal static class SimulationIO
{
    public static LogChannel LogChannel = Log.CreateChannel("Simulation", activeByDefault: false);
}
#pragma warning restore IDE1006 // Naming Styles