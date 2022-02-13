using CCC.Fix2D;
using CCC.InspectorDisplay;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class ActionOnContactAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public enum ContactMode
    {
        OnColliderContact,
        OnOverlap
    }

    [System.Serializable]
    public class Element
    {
        public GameActionAuth ActionPrefab = null;
        public ActorFilter TargetFilter = ActorFilter.Enemies;
        public float SameTargetCooldown = 0f;
        public ContactMode ContactMode = ContactMode.OnColliderContact;

        [ShowIf(nameof(IsContactModeOverlap))]
        public LayerMask OverlapLayerMask;
        [ShowIf(nameof(IsContactModeOverlap))]
        public float OverlapRadius = 0.5f;

        private bool IsContactModeOverlap => ContactMode == ContactMode.OnOverlap;
    }

    public List<Element> Actions = new List<Element>();

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        for (int i = 0; i < Actions.Count; i++)
        {
            var item = Actions[i];
            if (item.ActionPrefab == null)
                continue;

            var baseData = new ActionOnContactBaseData()
            {
                ActionEntity = conversionSystem.GetPrimaryEntity(item.ActionPrefab.gameObject),
                ActionFilter = item.TargetFilter,
                Id = (byte)i,
                SameTargetCooldown = (fix)item.SameTargetCooldown,
            };

            switch (item.ContactMode)
            {
                case ContactMode.OnColliderContact:
                {
                    var buffer = dstManager.GetOrAddBuffer<ActionOnColliderContact>(entity);
                    buffer.Add(new ActionOnColliderContact()
                    {
                        Data = baseData
                    }); ;
                    break;
                }

                case ContactMode.OnOverlap:
                {
                    var buffer = dstManager.GetOrAddBuffer<ActionOnOverlap>(entity);
                    buffer.Add(new ActionOnOverlap()
                    {
                        Data = baseData,
                        OverlapFilter = (uint)item.OverlapLayerMask.value,
                        OverlapRadius = (fix)item.OverlapRadius,
                    });
                    break;
                }
            }
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (var item in Actions)
        {
            if (item.ActionPrefab == null)
                continue;

            referencedPrefabs.Add(item.ActionPrefab.gameObject);
        }
    }
}