using UnityEngine;

public class ReadOnlyInEditMode : PropertyAttribute
{
    public readonly bool forwardToChildren = true;
    public ReadOnlyInEditMode(bool forwardToChildren = true) { this.forwardToChildren = forwardToChildren; }
}
