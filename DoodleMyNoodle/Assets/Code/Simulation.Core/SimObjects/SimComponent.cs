using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SimComponent : SimObject
{
    protected SimComponent() { }

    public SimEntity entity { get; internal set; }

    public override string ToString()
    {
        return $"<{GetType()}> on entity {(entity == null ? "" : entity.name)}";
    }
}
