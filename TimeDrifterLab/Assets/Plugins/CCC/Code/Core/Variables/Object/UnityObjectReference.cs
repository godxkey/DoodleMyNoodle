using UnityEngine;

[System.Serializable]
public class UnityObjectReference : VarReference<UnityObjectVariable, Object>
{
    public UnityObjectReference() : base()
    { }

    public UnityObjectReference(Object value) : base(value)
    { }
}