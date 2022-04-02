using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

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

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(.6f, 1f, .6f, 0.5f);
        for (int i = 0; i < 4; i++)
        {
            float charRadius = (float)SimulationGameConstants.CharacterRadius;
            Gizmos.DrawWireSphere(transform.position - new Vector3(i * MemberSpacing, 0), charRadius);
        }

#if UNITY_EDITOR
        string gs_CameraAssetGUID = "a8ff186fd45970d4f879b4997444b57f";
        var gs_Camera = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(UnityEditor.AssetDatabase.GUIDToAssetPath(gs_CameraAssetGUID));

        if (gs_Camera != null)
        {
            var camController = gs_Camera.GetComponent<CameraController>();
            if (camController != null)
            {
                Gizmos.DrawWireCube((Vector2)transform.position + camController.OffsetFromGroupPosition, size: new Vector3(camController.DesiredWidth, camController.DesiredWidth * 9f / 16f, 0));
            }
            else
            {
                Log.Warning($"Could not find CameraController component on asset {gs_Camera.name}");
            }
        }
        else
        {
            Log.Warning($"Could not find GS_Camera asset with guid {gs_CameraAssetGUID}");
        }

#endif
    }
}