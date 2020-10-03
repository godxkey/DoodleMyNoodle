using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using static fixMath;

public class HealthDisplayManagementSystem : GamePresentationBehaviour
{
    public GameObject HealthBarPrefab;

    private List<GameObject> _healthBarInstances = new List<GameObject>();

    protected override void OnGamePresentationUpdate()
    {
        int healthBarAmount = 0;
        SimWorldCache.SimWorld.Entities.ForEach((Entity pawn, ref Health entityHealth, ref MaximumInt<Health> entityMaximumHealth, ref FixTranslation entityTranslation) =>
        {
            Entity pawnController = CommonReads.GetPawnController(SimWorldCache.SimWorld, pawn);

            // shell is empty, no healthbar
            if (pawnController == Entity.Null)
            {
                return;
            }

            // Remove healthbar for self
            if (pawnController == SimWorldCache.LocalController)
            {
                return;
            }

            fix healthRatio = (fix)entityHealth.Value / (fix)entityMaximumHealth.Value;

            Team LocalPlayerTeam = SimWorldCache.SimWorld.GetComponentData<Team>(SimWorldCache.LocalController);
            Team CurrentPawnTeam = SimWorldCache.SimWorld.GetComponentData<Team>(pawnController);

            SetOrAddHealthBar(healthBarAmount, entityTranslation.Value, healthRatio, LocalPlayerTeam.Value == CurrentPawnTeam.Value);

            healthBarAmount++;
        });

        // Deactivate extra HealthBar
        for (int i = healthBarAmount; i < _healthBarInstances.Count; i++)
        {
            _healthBarInstances[i].SetActive(false);
        }
    }

    private void SetOrAddHealthBar(int index, fix3 position, fix ratio, bool friendly)
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

        currentHealthBar.transform.position = (position + new fix3(0,fix(0.6f),fix(-0.1f))).ToUnityVec();
        currentHealthBar.GetComponent<UIBarDisplay>()?.AjustDisplay((float)ratio);
        currentHealthBar.GetComponent<UIBarDisplay>()?.ChangeColor(friendly ? Color.green : Color.red);
        currentHealthBar.SetActive(true);
    }
}