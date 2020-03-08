using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimVelocityComponent : SimComponent, ISimTickable
{
    [System.Serializable]
    struct SerializedData
    {
        public FixVector2 Velocity;
    }

    public FixVector2 Value { get => _data.Velocity; set => _data.Velocity = value; }

    public void OnSimTick()
    {
        SimTransform.LocalPosition += _data.Velocity * Simulation.DeltaTime;
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [CCC.InspectorDisplay.AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        // define default values here
    };

    public override void PushToDataStack(SimComponentDataStack dataStack)
    {
        base.PushToDataStack(dataStack);
        dataStack.Push(_data);
    }

    public override void PopFromDataStack(SimComponentDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();
        base.PopFromDataStack(dataStack);
    }
    #endregion
}
