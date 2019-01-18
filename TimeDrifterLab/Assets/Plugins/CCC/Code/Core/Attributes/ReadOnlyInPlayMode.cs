using UnityEngine;

public class ReadOnlyInPlayMode : PropertyAttribute
{
    public readonly bool forwardToChildren = true;
    public ReadOnlyInPlayMode(bool forwardToChildren = true) { this.forwardToChildren = forwardToChildren; }
}
