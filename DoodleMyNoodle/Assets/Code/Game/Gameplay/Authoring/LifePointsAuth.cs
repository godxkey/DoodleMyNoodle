using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class LifePointsAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public int Quantity = 5;
    public List<GameActionAuth> OnLossGameActions = new List<GameActionAuth>();

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<LifePoints>(entity, Quantity);

        if (OnLossGameActions.Count > 0)
        {
            var buffer = dstManager.AddBuffer<LifePointLostAction>(entity);

            foreach (var actionAuth in OnLossGameActions)
            {
                if (actionAuth != null)
                    buffer.Add(conversionSystem.GetPrimaryEntity(actionAuth));
            }
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (var actionAuth in OnLossGameActions)
        {
            if (actionAuth != null)
                referencedPrefabs.Add(actionAuth.gameObject);
        }
    }
}