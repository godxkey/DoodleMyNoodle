using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SimPlayerStatsComponent : SimComponent
{
    public int StartHealthpoints = 10;

    public Stat<int> Healthpoints;
    
    public int GetCurrentHealthpoints { get { return Healthpoints.GetValue().Value; } }

    [System.Serializable]
    public class OnHPChanged : UnityEvent<float, float> { }
    public OnHPChanged OnHealthpointsChanged = new OnHPChanged();

    public override void OnSimStart()
    {
        base.OnSimStart();

        Healthpoints = new Stat<int>(StartHealthpoints);
    }

    public int ChangeValue(int value)
    {
        Healthpoints.SetValue(Healthpoints.GetValue().Value + value);

        if (Healthpoints.GetValue().IsDirty)
        {
            OnHealthpointsChanged?.Invoke(Healthpoints.GetValue().Value, StartHealthpoints);
        }

        return Healthpoints.GetValue().Value - Healthpoints.GetValue().PreviousValue;
    }
}
