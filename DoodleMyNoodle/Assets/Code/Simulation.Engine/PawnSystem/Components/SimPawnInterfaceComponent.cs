using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An entity with this can be controlled
/// </summary>
public class SimPawnInterfaceComponent : SimComponent
{
    List<ISimPawnInputHandler> _cachedComponentList = new List<ISimPawnInputHandler>(); // this is simply used to reduce allocations
    public void HandleInput(SimInput input)
    {
        // forward inputs to 'pawn input handlers'
        GetComponents<ISimPawnInputHandler>(_cachedComponentList);

        for (int i = 0; i < _cachedComponentList.Count; i++)
        {
            if (_cachedComponentList[i].HandleInput(input))
            {
                break;
            }
        }

        _cachedComponentList.Clear();
    } 
}