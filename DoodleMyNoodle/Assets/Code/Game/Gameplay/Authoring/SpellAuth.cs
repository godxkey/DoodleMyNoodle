using Unity.Entities;
using UnityEngine;
using CCC.InspectorDisplay;
using UnityEngineX.InspectorDisplay;

[DisallowMultipleComponent]
[RequireComponent(typeof(SimAsset))]
public class SpellAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    // Sim
    [Suffix("sec")]
    public float Cooldown = 0;

    // Presentation
    public bool OverrideDescription;
    [ShowIf(nameof(OverrideDescription))]
    public TextData DescriptionOverride;

    public bool OverrideIcon;
    [ShowIf(nameof(OverrideIcon))]
    public Sprite IconOverride;

    public AudioPlayable BeginCastSFX;
    public SurveyBaseController CastSurvey;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new SpellCooldown() { Value = (fix)Cooldown });
    }
}