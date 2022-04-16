using UnityEngine;
using UnityEngineX;
using DG.Tweening;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using static fixMath;
using CCC.Fix2D;

public class DamageEventDisplaySystem : GamePresentationSystem<DamageEventDisplaySystem>
{
    [Header("VFX")]
    public GameObject DamageImpactVFX;
    public GameObject HealImpactVFX;

    [Header("Number Settings")]
    public float NumberScaleForLocalPlayer = 1.25f;
    public float NumberScaleForAllies = 0.8f;
    public float NumberAlphaForAllies = 1f;
    [Tooltip("The impact position of the damage/heal is often at the edge of the collider. We usually want to move the vfx a bit closer to the character center." +
        " This value will affect the distance we move the VFX position. 0 means we don't move the VFX at all. 1 means we will move the VFX up to 1 unit towards the center.")]
    public float NumberInsetDistanceFromImpact = 0.2f;
    [Tooltip("The radius of the random used for the final position of the number + impact VFX")]
    public float NumberPositionRandomRadius = 0.1f;

    public override void PresentationUpdate()
    {
        foreach (var healthDeltaEvent in PresentationEvents.HealthDeltaEvents.SinceLastPresUpdate)
        {
            ProcessEvent(healthDeltaEvent);
        }
    }

    private void ProcessEvent(HealthDeltaEventData eventData)
    {
        Vector2 victimPos = eventData.VictimPosition.ToUnityVec();
        if (SimWorld.TryGetComponent(eventData.Victim, out FixTranslation victimLatestTranslation))
        {
            victimPos = (Vector2)victimLatestTranslation.Value;
        }
        float targetRadius = (float)CommonReads.GetActorRadius(SimWorld, eventData.Victim);
        // clamp impact vector to victim's radius (+ threshold)
        Vector2 victimImpactOffset = Vector2.ClampMagnitude((Vector2)eventData.ImpactVector, maxLength: targetRadius - NumberInsetDistanceFromImpact);
        victimImpactOffset += new Vector2(Random.Range(-NumberPositionRandomRadius, NumberPositionRandomRadius), Random.Range(-NumberPositionRandomRadius, NumberPositionRandomRadius));

        Vector2 displayedImpactPos = victimPos + victimImpactOffset;

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Request Number
        ////////////////////////////////////////////////////////////////////////////////////////
        {
            fix displayedValue;
            Color color;
            Vector2 scale;

            if (eventData.IsHeal)
            {
                // Heal
                displayedValue = max(1, round(eventData.TotalUncappedDelta));
                color = Color.green;
            }
            else
            {
                // Damage
                displayedValue = max(1, round(-eventData.TotalUncappedDelta));
                color = Color.white;
            }

            if (eventData.InstigatorSet.FirstPhysicalInstigator == PlayerHelpers.GetLocalSimPawnEntity(SimWorld))
            {
                scale = Vector2.one * NumberScaleForLocalPlayer;
            }
            else
            {
                scale = Vector2.one * NumberScaleForAllies;
                color.a = eventData.IsAutoAttack ? 0 : NumberAlphaForAllies;
            }

            GameSystem<FloatingTextSystem>.Instance.RequestText(displayedImpactPos, scale, displayedValue.ToString(), color);
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Impact VFX
        ////////////////////////////////////////////////////////////////////////////////////////
        if (eventData.IsDamage)
        {
            if (DamageImpactVFX)
                Instantiate(DamageImpactVFX, displayedImpactPos, Quaternion.identity).Destroy(4f);
        }
        else
        {
            if (HealImpactVFX)
                Instantiate(HealImpactVFX, displayedImpactPos, Quaternion.identity).Destroy(4f);
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Flash Sprite
        ////////////////////////////////////////////////////////////////////////////////////////
        //if (eventData.IsDamage
        //    && BindedSimEntityManaged.InstancesMap.TryGetValue(eventData.AffectedEntity, out GameObject presentationEntity)
        //    && presentationEntity)
        //{
        //    DoodleDisplay playerCharacterDoodleDisplay = presentationEntity.GetComponent<DoodleDisplay>();
        //    SpriteRenderer entityRenderer = null;
        //    if (playerCharacterDoodleDisplay != null)
        //    {
        //        entityRenderer = playerCharacterDoodleDisplay.SpriteRenderer;
        //    }
        //    else
        //    {
        //        entityRenderer = presentationEntity.GetComponentInChildren<SpriteRenderer>();
        //    }

        //    if (entityRenderer == null)
        //    {
        //        Log.Error("No Sprite Rendered Found on Entity VE");
        //        return;
        //    }

        //    Sequence sq = DOTween.Sequence();
        //    sq.AppendCallback(() => { if (entityRenderer) entityRenderer.SetAlpha(0); });
        //    sq.AppendInterval(0.1f);
        //    sq.AppendCallback(() => { if (entityRenderer) entityRenderer.SetAlpha(1); });
        //    sq.AppendInterval(0.1f);
        //    sq.SetLoops(6);
        //}
    }
}