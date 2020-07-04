using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class HealthPoolDisplay : GamePresentationBehaviour
{
    public Image HealthPool;
    public Image ArmorPool;

    protected override void OnGamePresentationUpdate()
    {
        Health hp = SimWorld.GetComponentData<Health>(SimWorldCache.LocalPawn);
        MaximumInt<Health> maxHP = SimWorld.GetComponentData<MaximumInt<Health>>(SimWorldCache.LocalPawn);
        Armor armor = SimWorld.GetComponentData<Armor>(SimWorldCache.LocalPawn);

        int totalHealth = maxHP.Value + armor.Value;

        HealthPool.fillAmount = (float)hp.Value / totalHealth;
        ArmorPool.fillAmount = (float)(hp.Value + armor.Value) / totalHealth;
    }
}