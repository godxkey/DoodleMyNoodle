using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using CCC.InspectorDisplay;

public class SimStatComponent : SimComponent
{
    [System.Serializable]
    struct SerializedData
    {
        public Stat<int> Stat;
        public int StartValue;
    }

    private Stat<int> Stat { get => _dataSimStatComponent.Stat; set => _dataSimStatComponent.Stat = value; }
    public int StartValue { get => _dataSimStatComponent.StartValue; set => _dataSimStatComponent.StartValue = value; }
    public int Value { get { return Stat.Value; } }

    // Stat Changed Callback - New Value / Previous Value / Value on Start aka Max Value for now
    [System.Serializable]
    public class OnValueChanged : UnityEvent<float, float, float> { }
    [HideInInspector]
    public OnValueChanged OnStatChanged = new OnValueChanged();

    public override void OnSimAwake()
    {
        base.OnSimAwake();

        Stat = new Stat<int>(StartValue);
    }

    /// <summary>
    /// Returns the delta the stat value performed
    /// </summary>
    public int IncreaseValue(int value)
    {
        return SetValue(Stat.Value + value);
    }

    /// <summary>
    /// Returns the delta the stat value performed
    /// </summary>
    public int DecreaseValue(int value)
    {
        return SetValue(Stat.Value - value);
    }

    /// <summary>
    /// Returns the delta the stat value performed
    /// </summary>
    public virtual int SetValue(int value)
    {
        var currentStat = Stat;
        currentStat.SetValue(value);
        Stat = currentStat;

        if (Stat.HasChanged())
        {
            OnStatChanged?.Invoke(Stat.Value, Stat.PreviousValue, StartValue);
        }

        return Stat.Value - Stat.PreviousValue;
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
    SerializedData _dataSimStatComponent = new SerializedData()
    {
        // define default values here
    };

    public override void PushToDataStack(SimComponentDataStack dataStack)
    {
        base.PushToDataStack(dataStack);
        dataStack.Push(_dataSimStatComponent);
    }

    public override void PopFromDataStack(SimComponentDataStack dataStack)
    {
        _dataSimStatComponent = (SerializedData)dataStack.Pop();
        base.PopFromDataStack(dataStack);
    }
    #endregion
}
