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

        Vector2 displayedImpactPos;
        var victimView = PresentationHelpers.FindBindedView(eventData.Victim);
        if (victimView != null && victimView.TryGetComponent(out Collider2D viewCollider2D))
        {
            Vector2 viewPos = (Vector2)victimView.transform.position;
            Vector2 potentialImpactPos = viewPos + (Vector2)eventData.ImpactVector;
            if (!viewCollider2D.OverlapPoint(potentialImpactPos))
            {
                displayedImpactPos = viewCollider2D.ClosestPoint(potentialImpactPos);
            }
            else
            {
                displayedImpactPos = potentialImpactPos;
            }
        }
        else
        {
            // clamp impact vector to victim's radius (+ threshold)
            float targetRadius = (float)CommonReads.GetActorRadius(SimWorld, eventData.Victim);
            Vector2 victimImpactOffset = Vector2.ClampMagnitude((Vector2)eventData.ImpactVector, maxLength: targetRadius);
            displayedImpactPos = victimPos + victimImpactOffset;
        }

        displayedImpactPos += new Vector2(Random.Range(-NumberPositionRandomRadius, NumberPositionRandomRadius), Random.Range(-NumberPositionRandomRadius, NumberPositionRandomRadius));

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