using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimWorld
{
    ////////////////////////////////////////////////////////////////////////////////////////
    //      Everything in here should be synchronized across players. 
    //      If we ever save a game, that's all the data we should save.                                 
    ////////////////////////////////////////////////////////////////////////////////////////

    // the next tick the simulation is going to perform.
    internal uint tickId = 0;

    // the list of all entities in the simulation
    internal List<SimEntity> entities = new List<SimEntity>();

    // should start at 0 for all games
    internal SimEntityId nextEntityId = SimEntityId.firstValid;

    // fbessette:   since the "join in progress and download the sim from the server" feature is not implemented yet, 
    //              the seed will always be at 0 for all players. This means pressing "play" will always give the same random results at first.
    internal int seed = 0;
}
