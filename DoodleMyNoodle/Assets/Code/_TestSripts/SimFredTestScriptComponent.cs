using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SimFredTestScriptComponent : SimComponent, ISimTickable
{
    [System.Serializable]
    struct SerializedData
    {
    }

    public override void OnSimAwake()
    {
        base.OnSimAwake();
    }

    public void OnSimTick()
    {
    }


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
