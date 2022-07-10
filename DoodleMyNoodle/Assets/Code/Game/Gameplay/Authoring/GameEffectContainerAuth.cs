using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class GameEffectContainerAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public List<GameObject> GameEffects;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<GameEffectBufferElement>(entity);

        foreach (var gameEffect in GameEffects)
        {
            var buffer = dstManager.GetOrAddBuffer<GameEffectStartBufferElement>(entity);
            buffer.Add(new GameEffectStartBufferElement()
            {
                EffectEntity = conversionSystem.GetPrimaryEntity(gameEffect)
            });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (var gameEffect in GameEffects)
        {
            if (gameEffect == null)
                continue;

            referencedPrefabs.Add(gameEffect);
        }
    }
}