using UnityEngine;

[CreateAssetMenu(menuName = "DoodleMyNoodle/VFX/Item Used VFX")]
public class ItemUsedVFXDefinition : VFXDefinition
{
    public float DisplacementX = 0;
    public float DisplacementY = 0;

    protected override void OnTriggerVFX()
    {
        Vector3 location = GetVFXData<Vector2>("Location");
        Sprite sprite = GetVFXData<Sprite>("Sprite");
        location += new Vector3(DisplacementX, DisplacementY, 0);
        GameObject newVFX = Instantiate(VFXToSpawn, location, Quaternion.identity);
        if (newVFX.TryGetComponent(out SpriteRendererScalingFromTexture spriteRendererScalingFromTexture))
        {
            spriteRendererScalingFromTexture.SetScalingFromTexture(sprite);
        }
    }
}