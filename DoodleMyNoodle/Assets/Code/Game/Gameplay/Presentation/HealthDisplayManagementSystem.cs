using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using static fixMath;

public class HealthDisplayManagementSystem : GameMonoBehaviour
{
    public GameObject HealthBarPrefab;

    private List<GameObject> _healthBarInstances = new List<GameObject>();

    public override void OnGameUpdate() 
    {
        int healthBarAmount = 0;
        GameMonoBehaviourHelpers.GetSimulationWorld().Entities.ForEach((ref Health entityHealth, ref MaximumInt<Health> entityMaximumHealth, ref FixTranslation entityTranslation)=>
        {
            fix healthRatio = (fix)entityHealth.Value / (fix)entityMaximumHealth.Value;

            SetOrAddHealthBar(healthBarAmount, entityTranslation.Value, healthRatio);

            healthBarAmount++;
        });

        // Deactivate extra HealthBar
        for (int i = healthBarAmount; i < _healthBarInstances.Count; i++)
        {
            _healthBarInstances[i].SetActive(false);
        }
    }

    private void SetOrAddHealthBar(int index, fix3 position, fix ratio)
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

        currentHealthBar.transform.position = (position + new fix3(0,fix(0.6f),fix(-0.1f))).ToUnityVec();
        currentHealthBar.GetComponent<UIBarDisplay>()?.AjustDisplay((float)ratio);
        currentHealthBar.SetActive(true);
    }
}