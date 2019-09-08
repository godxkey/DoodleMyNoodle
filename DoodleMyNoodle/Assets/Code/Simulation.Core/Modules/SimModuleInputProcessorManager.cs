using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimModuleInputProcessorManager : IDisposable
{
    List<ISimInputProcessor> _inputProcessors = new List<ISimInputProcessor>();
    int _inputProcessorIterator;

    internal void ProcessInput(SimInput input)
    {
        if (input is SimCommand simCommand)
        {
            simCommand.Execute();
        }
        else
        {
            // TODO
            // TEMPORAIRE
            for (_inputProcessorIterator = 0; _inputProcessorIterator < _inputProcessors.Count; _inputProcessorIterator++)
            {
                ISimInputProcessor processor = _inputProcessors[_inputProcessorIterator];
                if (processor.isActiveAndEnabled)
                {
                    try
                    {
                        processor.ProcessInput(input);
                    }
                    catch (Exception e)
                    {
                        DebugService.LogError(e.Message + " - stack:\n " + e.StackTrace);
                    }
                }
            }
        }
    }

    internal void OnAddSimObjectToSim(SimObject obj)
    {
        if (obj is ISimInputProcessor simInputProcessor)
        {
            _inputProcessors.Add(simInputProcessor);
        }
    }

    internal void OnRemovingSimObjectFromSim(SimObject obj)
    {
        if (obj is ISimInputProcessor simInputProcessor)
        {
            int i = _inputProcessors.IndexOf(simInputProcessor);
            _inputProcessors.RemoveAt(i);
            if (_inputProcessorIterator >= i)
                _inputProcessorIterator--;
        }
    }

    public void Dispose()
    {
    }
}
