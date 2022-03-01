using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerGroupAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public float MemberSpacing = 1f;
    public Vector2 StopTriggerSize = Vector2.one;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<PlayerGroupDataTag>(entity);
        dstManager.AddComponentData(entity, new PlayerGroupSpacing() { Value = (fix)MemberSpacing });
        dstManager.AddComponentData(entity, new PlayerGroupStopTriggerSize() { Value = (fix2)StopTriggerSize });
        dstManager.AddComponentData<Team>(entity, (int)DesignerFriendlyTeam.Player);
    }

    private void OnDrawGizmosSelected()
    {
        float horizontalOffset = (float)SimulationGameConstants.CharacterRadius + 0.05f;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + new Vector3(StopTriggerSize.x / 2f + horizontalOffset, 0), StopTriggerSize);
    }
}