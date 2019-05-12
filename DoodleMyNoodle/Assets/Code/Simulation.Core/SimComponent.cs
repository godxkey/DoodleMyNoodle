using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SimComponent : SimObject
{
    internal SimComponent() { }


    public SimEntity entity { get; private set; }

    public void Initialize(SimEntity owner)
    {
        entity = owner;
    }
}
