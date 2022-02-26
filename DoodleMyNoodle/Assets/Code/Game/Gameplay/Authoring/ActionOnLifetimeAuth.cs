using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class ActionOnLifetimeAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [System.Serializable]
    public class Entry
    {
        public GameActionAuth Action;
        public float AtLifetime;
    }

    public List<Entry> Entries = new List<Entry>();

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var buffer = dstManager.AddBuffer<ActionOnLifetime>(entity);

        foreach (var item in Entries)
        {
            if (item.Action != null)
            {
                buffer.Add(new ActionOnLifetime()
                {
                    Action = conversionSystem.GetPrimaryEntity(item.Action.gameObject),
                    Lifetime = (fix)item.AtLifetime
                });
            }
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (var item in Entries)
        {
            if (item.Action != null)
            {
                referencedPrefabs.Add(item.Action.gameObject);
            }
        }
    }
}