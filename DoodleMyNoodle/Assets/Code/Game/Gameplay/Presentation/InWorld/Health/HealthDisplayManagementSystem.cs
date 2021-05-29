using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using CCC.Fix2D;
using static fixMath;

public class HealthDisplayManagementSystem : GamePresentationSystem<HealthDisplayManagementSystem>
{
    public GameObject HealthBarPrefab;

    private List<GameObject> _healthBarInstances = new List<GameObject>();

    protected override void OnGamePresentationUpdate()
    {
        int healthBarAmount = 0;
        
        Team localPlayerTeam = Cache.LocalControllerTeam;

        Cache.SimWorld.Entities.ForEach((Entity pawn, ref Health entityHealth, ref MaximumInt<Health> entityMaximumHealth, ref FixTranslation entityTranslation) =>
        {
            Entity pawnController = CommonReads.GetPawnController(Cache.SimWorld, pawn);

            // shell is empty, no healthbar
            if (pawnController == Entity.Null)
            {
                return;
            }

            Team CurrentPawnTeam = Cache.SimWorld.GetComponent<Team>(pawnController);

            SetOrAddHealthBar(healthBarAmount, entityTranslation.Value, entityMaximumHealth.Value, entityHealth.Value, localPlayerTeam.Value == CurrentPawnTeam.Value);

            healthBarAmount++;
        });

        // Deactivate extra HealthBar
        for (int i = healthBarAmount; i < _healthBarInstances.Count; i++)
        {
            _healthBarInstances[i].SetActive(false);
        }
    }

    private void SetOrAddHealthBar(int index, fix3 position, int maxHealth, int health, bool friendly)
    {
        GameObject currentHealthBar;
        if (_healthBarInstances.Count <= index)
        {
            currentHealthBar = Instantiate(HealthBarPrefab);
            _healthBarInstances.Add(currentHealthBar);
        }
        else
        {
            currentHealthBar = _healthBarInstances[index];
        }

        currentHealthBar.transform.position = (position + new fix3(0,fix(0.7f),0)).ToUnityVec();
        currentHealthBar.GetComponent<UIBarDisplay>()?.SetMaxHealth(maxHealth);
        currentHealthBar.GetComponent<UIBarDisplay>()?.SetHealth(health);
        currentHealthBar.SetActive(true);
    }
}