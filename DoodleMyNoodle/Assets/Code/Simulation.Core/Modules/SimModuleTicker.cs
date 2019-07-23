using System.Collections.Generic;

public class SimModuleTicker
{
    internal bool canSimBeTicked => SimModules.sceneLoader.pendingSceneLoads == 0
        && !isTicking;

    internal bool isTicking = false;
    internal uint tickId => SimModules.world.tickId;

    internal List<ISimTickable> tickables = new List<ISimTickable>();

    internal void Tick(in SimTickData tickData)
    {
        if (!canSimBeTicked)
            throw new System.Exception("Tried to tick the simulation while it could not. Investigate.");

        isTicking = true;

        ////////////////////////////////////////////////////////////////////////////////////////
        //      INPUTS                                 
        ////////////////////////////////////////////////////////////////////////////////////////
        foreach (SimInput input in tickData.inputs)
        {
            SimCommand simCommand = input as SimCommand;
            if (simCommand != null)
            {
                simCommand.Execute();
            }
            else
            {
                // TODO
                // TEMPORAIRE
                SimModules.worldSearcher.ForEveryEntityWithComponent<ISimInputHandler>((handler) =>
                {
                    if(handler is SimComponent c && !c.isActiveAndEnabled)
                    {
                        return true; // continue
                    }

                    bool handled = handler.HandleInput(input);
                    return !handled; // continue if not handled
                });
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      TICK                                 
        ////////////////////////////////////////////////////////////////////////////////////////
        for (int i = 0; i < tickables.Count; i++)
        {
            if (tickables[i].isActiveAndEnabled)
            {
                tickables[i].OnSimTick();
            }
        }

        SimModules.world.tickId++;


        isTicking = false;
    }

    internal void OnAddSimComponentToSim(SimComponent comp)
    {
        if (comp is ISimTickable)
        {
            tickables.Add((ISimTickable)comp);
        }
    }

    internal void OnRemovingSimComponentToSim(SimComponent comp)
    {
        if (comp is ISimTickable)
        {
            tickables.Remove((ISimTickable)comp);
        }
    }
}