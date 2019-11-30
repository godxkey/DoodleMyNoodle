using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Entity that can be controlled
/// </summary>
public class SimPawnComponent : SimComponent
{
    [System.Serializable]
    struct SerializedData
    {
        public SimPlayerId PlayerInControl;
    }

    public SimPlayerId PlayerInControl { get => _data.PlayerInControl; set => _data.PlayerInControl = value; }
    
    public bool IsPossessed => _data.PlayerInControl.IsValid;

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        // define default values here
    };

    public override void SerializeToDataStack(SimComponentDataStack dataStack)
    {
        base.SerializeToDataStack(dataStack);
        dataStack.Push(_data);
    }

    public override void DeserializeFromDataStack(SimComponentDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();
        base.DeserializeFromDataStack(dataStack);
    }
    #endregion
}