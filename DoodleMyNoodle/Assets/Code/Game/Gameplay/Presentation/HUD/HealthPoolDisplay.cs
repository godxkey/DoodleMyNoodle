using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class HealthPoolDisplay : GamePresentationBehaviour
{
    [SerializeField] GameObject _poolContainer;
    [SerializeField] Image _healthPool;
    [SerializeField] Image _armorPool;

    protected override void OnGamePresentationUpdate()
    {
        if(SimWorldCache.LocalPawn != Entity.Null)
        {
            _poolContainer.gameObject.SetActive(true);

            Health hp = SimWorld.GetComponentData<Health>(SimWorldCache.LocalPawn);
            MaximumInt<Health> maxHP = SimWorld.GetComponentData<MaximumInt<Health>>(SimWorldCache.LocalPawn);
            Armor armor = SimWorld.GetComponentData<Armor>(SimWorldCache.LocalPawn);

            int totalHealth = maxHP.Value + armor.Value;

            _healthPool.fillAmount = (float)hp.Value / totalHealth;
            _armorPool.fillAmount = (float)(hp.Value + armor.Value) / totalHealth;
        }
        else
        {
            _poolContainer.gameObject.SetActive(false);
        }
    }
}