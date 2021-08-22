using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/VFX/Melee Attack VFX")]
public class MeleeAttackVFXDefinition : VFXDefinition
{
    public Sprite WeaponSprite;
    public float DisplacementTowardsTarget = 1;

    protected override void OnTriggerVFX()
    {
        Vector3 DisplacementVector = GetVFXData<Vector2>("AttackVector").normalized * DisplacementTowardsTarget;
        Vector3 endPos = GetVFXData<Transform>("Transform").position + DisplacementVector;

        Vector2 attackDirection = GetVFXData<Vector2>("AttackVector");
        int AttackDirectionSign = 1;
        if (attackDirection.x > 0)
        {
            AttackDirectionSign *= -1;
        }

        GameObject newVFX = Instantiate(VFXToSpawn, endPos, Quaternion.identity);
        if (newVFX.TryGetComponent(out MeleeAttackVFX meleeAttackVFX))
        {
            meleeAttackVFX.StartMeleeAttackVFX(WeaponSprite, Duration, AttackDirectionSign);
        }
    }
}