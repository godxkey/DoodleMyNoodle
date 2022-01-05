using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/VFX/Melee Attack VFX")]
public class MeleeAttackVFXDefinition : VFXDefinition
{
    public Sprite WeaponSprite;
    public float DisplacementTowardsTarget = 1;
    public float Scale = 1;
    public bool SpawnOnInstigatorInstead = false;

    protected override void OnTriggerVFX()
    {
        Vector3 DisplacementVector = GetVFXData<fix2>("AttackVector").ToUnityVec().normalized * DisplacementTowardsTarget;
        Vector3 endPos = GetVFXData<Transform>("Transform").position + DisplacementVector;
        if (SpawnOnInstigatorInstead)
        {
            endPos = GetVFXData<Vector3>("InstigatorStartPosition") + DisplacementVector;
        }

        Vector2 attackDirection = GetVFXData<fix2>("AttackVector").ToUnityVec();
        attackDirection.Normalize();
        int AttackDirectionSign = 1;
        if (attackDirection.x > 0)
        {
            AttackDirectionSign *= -1;
        }

        GameObject newVFX = Instantiate(VFXToSpawn, endPos, Quaternion.identity);
        newVFX.transform.localScale = new Vector3(Scale, Scale, Scale);
        if (newVFX.TryGetComponent(out MeleeAttackVFX meleeAttackVFX))
        {
            meleeAttackVFX.StartMeleeAttackVFX(WeaponSprite, Duration, AttackDirectionSign);
        }
    }
}