using System.Collections.Generic;

public class SimModuleTicker
{
    internal bool canSimBeTicked => SimModules.sceneLoader.pendingSceneLoads == 0;

    internal List<ISimTickable> tickables = new List<ISimTickable>();

    internal void Tick(in SimTickData tickData)
    {
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
            }
        }

        SimModules.world.tickId++;

        for (int i = 0; i < tickables.Count; i++)
        {
            if (tickables[i].isActiveAndEnabled)
            {
                tickables[i].OnSimTick();
            }
        }
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