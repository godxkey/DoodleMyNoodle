using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimGridBattleBulletComponent : SimComponent, ISimTickable
{
    public FixVector2 Speed { get => _data.Speed; set => _data.Speed = value; }

    public void OnSimTick()
    {
        SimTransform.WorldPosition += (FixVector3)(Speed * Simulation.DeltaTime);
    }

    [System.Serializable]
    struct SerializedData
    {
        public FixVector2 Speed;

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
