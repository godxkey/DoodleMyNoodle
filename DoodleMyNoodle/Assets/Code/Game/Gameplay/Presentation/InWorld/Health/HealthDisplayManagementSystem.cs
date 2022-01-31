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
    public float HideDelayFromMouse = 0.4f;
    public float HideDelayFromDamageOrHeal = 1f;

    public int MaxHearthToDisplay = 10;

    private List<GameObject> _healthBarInstances = new List<GameObject>();

    private Dictionary<Entity, GameObject> EntitiesHPBar = new Dictionary<Entity, GameObject>();

    protected override void OnGamePresentationUpdate()
    {
        int healthBarAmount = 0;

        Team localPlayerTeam = Cache.DEPRECATED_LocalControllerTeam;

        Cache.SimWorld.Entities.ForEach((Entity entity, ref Health entityHealth, ref MaximumInt<Health> entityMaximumHealth, ref FixTranslation entityTranslation) =>
        {
            SetOrAddHealthBar(entity, healthBarAmount, entityTranslation.Value, entityMaximumHealth.Value, entityHealth.Value);

            healthBarAmount++;
        });

        // Deactivate extra HealthBar
        for (int i = healthBarAmount; i < _healthBarInstances.Count; i++)
        {
            _healthBarInstances[i].SetActive(false);
        }

        UpdateHealthDisplay();
    }

    private void UpdateHealthDisplay()
    {
        if (Cache.PointerInWorld && Cache.LocalPawn != Entity.Null)
        {
            foreach (var tileActor in Cache.PointedBodies)
            {
                if (SimWorld.TryGetComponent(tileActor, out Health health))
                {
                    if (health.Value > 0)
                    {
                        if (EntitiesHPBar.ContainsKey(tileActor))
                        {
                            EntitiesHPBar[tileActor].GetComponent<HealthBarDisplay>().Show(HideDelayFromMouse);
                        }
                    }
                }
            }
        }

        Cache.SimWorld.Entities.ForEach((ref DamageEventData damageEvent) =>
        {
            if (SimWorld.TryGetComponent(damageEvent.EntityDamaged, out FixTranslation position))
            {
                if (EntitiesHPBar.ContainsKey(damageEvent.EntityDamaged))
                {
                    EntitiesHPBar[damageEvent.EntityDamaged].GetComponent<HealthBarDisplay>().Show(HideDelayFromDamageOrHeal);
                }
            }
        });
    }

    private void SetOrAddHealthBar(Entity entity, int index, fix3 position, int maxHealth, int health)
    {
        GameObject currentHealthBar;
        if (_healthBarInstances.Count <= index)
        {
            currentHealthBar = Instantiate(HealthBarPrefab);
            _healthBarInstances.Add(currentHealthBar);
            if (EntitiesHPBar.ContainsKey(entity))
            {
                EntitiesHPBar[entity] = currentHealthBar;
            }
            else
            {
                EntitiesHPBar.Add(entity, currentHealthBar);
            }
        }
        else
        {
            currentHealthBar = _healthBarInstances[index];
            EntitiesHPBar[entity] = currentHealthBar;
        }

        currentHealthBar.transform.position = (position + new fix3(0, fix(0.7f), 0)).ToUnityVec();
        currentHealthBar.GetComponent<HealthBarDisplay>()?.SetHealth(health, maxHealth, MaxHearthToDisplay);
        currentHealthBar.SetActive(true);
    }
}