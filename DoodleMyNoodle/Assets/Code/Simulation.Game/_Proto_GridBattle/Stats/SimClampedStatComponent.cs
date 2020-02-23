using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimClampedStatComponent : SimStatComponent
{
    [System.Serializable]
    struct SerializedData
    {
        public int MaxValue;
        public int MinValue;
    }
    public int MaxValue { get => _dataSimClampedStatComponent.MaxValue; set => _dataSimClampedStatComponent.MaxValue = value; }
    public int MinValue { get => _dataSimClampedStatComponent.MinValue; set => _dataSimClampedStatComponent.MinValue = value; }

    public void SetClampedStatComponentValues(int maxValue, int minValue)
    {
        this.MaxValue = maxValue;
        this.MinValue = minValue;
    }

    public override int SetValue(int value)
    {
        int realValue = Mathf.Clamp(value, MinValue, MaxValue);
        return base.SetValue(realValue);
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
    SerializedData _dataSimClampedStatComponent = new SerializedData()
    {
        // define default values here
    };

    public override void PushToDataStack(SimComponentDataStack dataStack)
    {
        base.PushToDataStack(dataStack);
        dataStack.Push(_dataSimClampedStatComponent);
    }

    public override void PopFromDataStack(SimComponentDataStack dataStack)
    {
        _dataSimClampedStatComponent = (SerializedData)dataStack.Pop();
        base.PopFromDataStack(dataStack);
    }
    #endregion
}
