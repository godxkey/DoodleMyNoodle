using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/VFX/Lifespan VFX")]
public class LifespanVFXDefinition : VFXDefinition
{
    protected override void OnTriggerVFX()
    {
        Vector3 Location = GetVFXData<Vector2>("Location");
        GameObject newVFX = Instantiate(VFXToSpawn, Location, Quaternion.identity);
        if (newVFX.TryGetComponent(out LifespanVFX lifespanVFX))
        {
            lifespanVFX.StartVFX(Duration);
        }
    }
}
