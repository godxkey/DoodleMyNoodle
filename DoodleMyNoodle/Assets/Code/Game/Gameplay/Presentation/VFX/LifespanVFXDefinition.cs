using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/VFX/Lifespan VFX")]
public class LifespanVFXDefinition : VFXDefinition
{
    public float RandomDisplacementXMin = 0;
    public float RandomDisplacementXMax = 0;

    public float RandomDisplacementYMin = 0;
    public float RandomDisplacementYMax = 0;

    protected override void OnTriggerVFX()
    {
        Vector3 Location = GetVFXData<Vector2>("Location");

        Location += new Vector3(Random.Range(RandomDisplacementXMin, RandomDisplacementXMax), Random.Range(RandomDisplacementYMin, RandomDisplacementYMax), 0);

        GameObject newVFX = Instantiate(VFXToSpawn, Location, Quaternion.identity);
        if (newVFX.TryGetComponent(out LifespanVFX lifespanVFX))
        {
            lifespanVFX.StartVFX(Duration);
        }
    }
}