using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute
{
    public readonly bool ForwardToChildren = true;
    public ReadOnlyAttribute(bool forwardToChildren = true) { this.ForwardToChildren = forwardToChildren; }
}
