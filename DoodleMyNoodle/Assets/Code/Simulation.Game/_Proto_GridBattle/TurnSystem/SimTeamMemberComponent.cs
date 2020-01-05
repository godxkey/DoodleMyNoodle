using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimTeamMemberComponent : SimComponent
{
    public Team Team { get => _data.Team; set => _data.Team = value; }

    [System.Serializable]
    struct SerializedData
    {
        public Team Team;
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