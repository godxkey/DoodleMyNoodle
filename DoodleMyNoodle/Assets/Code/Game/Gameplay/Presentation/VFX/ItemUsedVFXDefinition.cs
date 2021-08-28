using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/VFX/Item Used VFX")]
public class ItemUsedVFXDefinition : VFXDefinition
{
    public float DisplacementX = 0;
    public float DisplacementY = 0;

    protected override void OnTriggerVFX()
    {
        Vector3 Location = GetVFXData<Vector2>("Location");
        Sprite Sprite = GetVFXData<Sprite>("Sprite");
        Location += new Vector3(DisplacementX, DisplacementY, 0);
        GameObject newVFX = Instantiate(VFXToSpawn, Location, Quaternion.identity);
        if (newVFX.TryGetComponent(out SpriteRendererScalingFromTexture SpriteRendererScalingFromTexture))
        {
            SpriteRendererScalingFromTexture.SetScalingFromTexture(Sprite);
        }
    }
}