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
        EntityQuery entityQuery = GameMonoBehaviourHelpers.SimulationWorld.CreateEntityQuery(ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<Health>());

        using (NativeArray<Entity> entities = entityQuery.ToEntityArray(Allocator.TempJob))
        {
            for (int i = 0; i < entities.Length; i++)
            {
                Entity entity = entities[i];

                Health entityHealth = GameMonoBehaviourHelpers.SimulationWorld.GetComponentData<Health>(entity);
                Maximum<Health> entityMaximumHealth = GameMonoBehaviourHelpers.SimulationWorld.GetComponentData<Maximum<Health>>(entity);
                Translation entityTranslation = GameMonoBehaviourHelpers.SimulationWorld.GetComponentData<Translation>(entity);

                Fix64 healthRatio = (Fix64)entityHealth.Value / (Fix64)entityMaximumHealth.Value;

                SetOrAddHealthBar(i, entityTranslation.Value, healthRatio);
            }
        }
    }

    private void SetOrAddHealthBar(int index, float3 position, Fix64 ratio)
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




