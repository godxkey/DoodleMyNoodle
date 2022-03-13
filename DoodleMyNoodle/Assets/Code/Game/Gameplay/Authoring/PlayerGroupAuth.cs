using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerGroupAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public float MemberSpacing = 1f;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<PlayerGroupDataTag>(entity);
        dstManager.AddComponentData(entity, new PlayerGroupSpacing() { Value = (fix)MemberSpacing });
        dstManager.AddComponentData<Team>(entity, (int)DesignerFriendlyTeam.Player);
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < 4; i++)
        {
            float charRadius = (float)SimulationGameConstants.CharacterRadius;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position - new Vector3(i * MemberSpacing, 0), charRadius);
        }
    }
}