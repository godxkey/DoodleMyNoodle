using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationTimeDisplay : GameMonoBehaviour
{
    public Text Text;
    public string Prefix = "SimTime: ";

    public override void OnGameLateUpdate()
    {
        Text.text = $"{Prefix}{GameMonoBehaviourHelpers.GetSimulationWorld().Time.ElapsedTime:F2}";
    }
}
