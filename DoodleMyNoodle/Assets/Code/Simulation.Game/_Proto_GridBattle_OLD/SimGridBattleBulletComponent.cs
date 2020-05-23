using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX.InspectorDisplay;

public class SimGridBattleBulletComponent : SimComponent, ISimTickable
{
    public fix2 Speed { get => _data.Speed; set => _data.Speed = value; }

    public void OnSimTick()
    {
        SimTransform.WorldPosition += (fix3)(Speed * Simulation.DeltaTime);
    }

    [System.Serializable]
    struct SerializedData
    {
        public fix2 Speed;

    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
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
