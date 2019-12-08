using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SimStatComponent : SimComponent
{
    private Stat<int> _stat;

    public int StartValue = 10;

    public int Value { get { return _stat.Value; } }

    // Stat Changed Callback - New Value / Previous Value / Value on Start aka Max Value for now
    [System.Serializable]
    public class OnValueChanged : UnityEvent<float, float, float> { }
    [HideInInspector]
    public OnValueChanged OnStatChanged = new OnValueChanged();

    public override void OnSimStart()
    {
        base.OnSimStart();

        _stat = new Stat<int>(StartValue);
    }

    /// <summary>
    /// Returns the delta the stat value performed
    /// </summary>
    public int IncreaseValue(int value)
    {
        return SetValue(_stat.Value + value);
    }

    /// <summary>
    /// Returns the delta the stat value performed
    /// </summary>
    public int DecreaseValue(int value)
    {
        return SetValue(_stat.Value - value);
    }

    /// <summary>
    /// Returns the delta the stat value performed
    /// </summary>
    public int SetValue(int value)
    {
        _stat.SetValue(value);
        if (_stat.HasChanged())
        {
            OnStatChanged?.Invoke(_stat.Value, _stat.PreviousValue, StartValue);
        }

        return _stat.Value - _stat.PreviousValue;
    }
}
