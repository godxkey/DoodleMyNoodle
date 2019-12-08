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
        _stat.SetValue(_stat.Value + value);

        if (_stat.HasChanged())
        {
            OnStatChanged?.Invoke(_stat.Value, _stat.PreviousValue, StartValue);
        }

        return _stat.Value - _stat.PreviousValue;
    }

    /// <summary>
    /// Returns the delta the stat value performed
    /// </summary>
    public int DecreaseValue(int value)
    {
        if (_stat.SetValue(_stat.Value - value))
        {
            OnStatChanged?.Invoke(_stat.Value, _stat.PreviousValue, StartValue);
        }

        return _stat.Value - _stat.PreviousValue;
    }

    /// <summary>
    /// Returns the delta the stat value performed
    /// </summary>
    public int SetValue(int value)
    {
        if (_stat.SetValue(value))
        {
            OnStatChanged?.Invoke(_stat.Value, _stat.PreviousValue, StartValue);
        }

        return _stat.Value - _stat.PreviousValue;
    }
}
