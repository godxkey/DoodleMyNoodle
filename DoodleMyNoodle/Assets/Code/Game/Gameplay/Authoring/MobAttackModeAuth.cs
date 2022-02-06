using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class MobAttackModeAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public enum EMode
    {
        Melee,
        WhenAboveOpponent,
    }

    public EMode Mode = EMode.Melee;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        switch (Mode)
        {
            case EMode.Melee:
                dstManager.AddComponent<MeleeAttackerTag>(entity);
                break;

            case EMode.WhenAboveOpponent:
                dstManager.AddComponent<DropAttackerTag>(entity);
                break;
        }
    }
}