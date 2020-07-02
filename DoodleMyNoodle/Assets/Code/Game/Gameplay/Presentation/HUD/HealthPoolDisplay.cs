using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class HealthPoolDisplay : GamePresentationBehaviour
{
    public Image HealthPool;
    public Image ArmorPool;

    public int HealthValue = 8;
    public int ArmorValue = 2;
    public int TotalValue = 10;

    protected override void OnGamePresentationUpdate()
    {
        HealthPool.fillAmount = (float)HealthValue / TotalValue;
        ArmorPool.fillAmount = (float)(HealthValue + ArmorValue) / TotalValue;
    }
}