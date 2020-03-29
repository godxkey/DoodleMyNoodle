using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class HealthDisplayManagementSystem : GameMonoBehaviour
{
    public GameObject HealthBarPrefab;

    private List<GameObject> _healthBarInstances = new List<GameObject>();

    public override void OnGameUpdate() 
    {
        int healthBarAmount = 0;
        GameMonoBehaviourHelpers.SimulationWorld.Entities.ForEach((ref Health entityHealth, ref MaximumInt<Health> entityMaximumHealth, ref Translation entityTranslation)=>
        {
            fix healthRatio = (fix)entityHealth.Value / (fix)entityMaximumHealth.Value;

            SetOrAddHealthBar(healthBarAmount, entityTranslation.Value, healthRatio);

            healthBarAmount++;
        });
    }

    private void SetOrAddHealthBar(int index, float3 position, fix ratio)
    {
        GameObject currentHealthBar = null;
        if (_healthBarInstances.Count <= index)
        {
            currentHealthBar = Instantiate(HealthBarPrefab);
            _healthBarInstances.Add(currentHealthBar);
        }
        else
        {
            currentHealthBar = _healthBarInstances[index];
        }

        currentHealthBar.transform.position = position + new float3(0,0.6f,-0.1f);
        currentHealthBar.GetComponent<UIBarDisplay>()?.AjustDisplay((float)ratio);
    }
}




