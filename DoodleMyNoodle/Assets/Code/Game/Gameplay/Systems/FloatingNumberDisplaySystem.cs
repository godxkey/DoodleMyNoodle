using System;
using UnityEngine;
using UnityEngineX;

public class FloatingNumberDisplaySystem : GamePresentationBehaviour
{
    public GameObject FloatingNumberPrefab;

    public override void OnPostSimulationTick()
    {
        SimWorldCache.SimWorld.Entities.ForEach((ref DamageAppliedEventData damageData) =>
        {
            // TODO : Do a pool system
            fix3 damagedEntityPosition = SimWorld.GetComponentData<FixTranslation>(damageData.EntityDamaged).Value;
            GameObject newFloatingNumber = Instantiate(FloatingNumberPrefab, damagedEntityPosition.ToUnityVec(), Quaternion.identity);
            newFloatingNumber.GetComponent<FloatingNumberDisplay>()?.Display(damageData.DamageApplied.ToString(), Color.red);
        });

        SimWorldCache.SimWorld.Entities.ForEach((ref HealingAppliedEventData healingData) =>
        {
            // TODO : Do a pool system
            fix3 damagedEntityPosition = SimWorld.GetComponentData<FixTranslation>(healingData.EntityHealed).Value;
            GameObject newFloatingNumber = Instantiate(FloatingNumberPrefab, damagedEntityPosition.ToUnityVec(), Quaternion.identity);
            newFloatingNumber.GetComponent<FloatingNumberDisplay>()?.Display(healingData.HealApplied.ToString(), Color.green);
        });
        
    }

    protected override void OnGamePresentationUpdate() { }
}