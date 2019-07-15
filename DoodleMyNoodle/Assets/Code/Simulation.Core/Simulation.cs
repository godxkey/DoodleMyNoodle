using System;
using System.Collections;
using System.Collections.Generic;

public partial class Simulation : IDisposable
{
    private static Simulation _instance;
    public Simulation() { _instance = this; }
    public void Dispose()
    {
        _world.Dispose();
        if (_instance == this)
            _instance = null;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public ISimBlueprintBank _blueprintBank;
    public SimWorld _world;
    public uint _tickId = 0;

    public void ChangeWorld(SimWorld world)
    {
        _world?.Dispose();
        _world = world;
    }

    public static bool isValid => _instance != null;
    public static ISimBlueprintBank blueprintBank => _instance?._blueprintBank;
    public static SimWorld world => _instance?._world;
    public static uint tickId => _instance == null ? 0 : _instance._tickId;

    public static void Tick(SimTickData tickData)
    {
        foreach (SimInput input in tickData.inputs)
        {
            SimCommand simCommand = input as SimCommand;
            if (simCommand != null)
            {
                simCommand.Execute(world);
            }
            else
            {
                // TODO
            }
        }

        world.Tick_PostInput();
        _instance._tickId++;
    }

    public static readonly Fix64 deltaTime = SimulationConstants.TIME_STEP; // 50 ticks per seconds
}
