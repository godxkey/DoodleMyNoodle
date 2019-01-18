using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute
{
    public readonly bool forwardToChildren = true;
    public ReadOnlyAttribute(bool forwardToChildren = true) { this.forwardToChildren = forwardToChildren; }
}
