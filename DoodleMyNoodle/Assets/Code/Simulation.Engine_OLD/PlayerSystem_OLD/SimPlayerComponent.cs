using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimPlayerComponent : SimComponent
{
    // The player id is derived from the entity id. Maybe we could remove SimPlayerId altogether and use the entity id directly ?
    public SimPlayerId SimPlayerId => new SimPlayerId(SimEntity.SimObjectId.Value);
}
