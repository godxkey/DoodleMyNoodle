using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX.InspectorDisplay;

[DisallowMultipleComponent]
public class AutoAttackAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [Suffix("/s")]
    public float AttackRate = 1;
    public GameActionAuth AttackAction = null;
    [Tooltip("If set to true, the attack progress will increase (up to 1) even if the actor is not in position for attack. When the pawn will be in position for attack, it will be able to fire earlier.")]
    public bool PrepareAttackInAdvance = false;
    [Tooltip("Set to -1 for no limit")]
    public int AttackLimit = -1;


    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<AutoAttackRate>(entity, (fix)AttackRate);
        dstManager.AddComponentData<AutoAttackProgress>(entity, (fix)(PrepareAttackInAdvance ? 1 : 0));
        dstManager.AddComponentData<ShouldAutoAttack>(entity, default);
        dstManager.AddComponentData<ProgressAutoAttackInAdvance>(entity, PrepareAttackInAdvance);
        dstManager.AddComponentData<RemainingAutoAttackCount>(entity, AttackLimit);
        dstManager.AddComponentData<AutoAttackAction>(entity, AttackAction != null ? conversionSystem.GetPrimaryEntity(AttackAction.gameObject) : default);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        if (AttackAction != null)
            referencedPrefabs.Add(AttackAction.gameObject);
    }
}
