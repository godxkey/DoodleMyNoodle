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
    [System.Serializable]
    public class NumberSetting
    {
        public float Scale = 1;
        public Color Color = Color.white;
    }

    [Header("VFX")]
    public GameObject DamageImpactVFX;
    public GameObject HealImpactVFX;

    [Header("Number Settings")]
    public NumberSetting SettingLocalAA;
    public NumberSetting SettingLocalSpell;
    public NumberSetting SettingAllyAA;
    public NumberSetting SettingAllySpell;
    public NumberSetting SettingGroupHealthLoss;
    public NumberSetting SettingGroupHealthGain;
    [Tooltip("The radius of the random used for the final position of the number + impact VFX")]
    public float NumberPositionRandomRadius = 0.1f;

    private enum Source
    {
        Enemy,
        Ally,
        LocalPlayer
    }

    private enum Type
    {
        Heal,
        Damage,
    }

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
        if (SimWorld.TryGetComponent(eventData.OriginalVictim, out FixTranslation victimLatestTranslation))
        {
            victimPos = (Vector2)victimLatestTranslation.Value;
        }

        Vector2 displayedImpactPos;
        var victimView = PresentationHelpers.FindBindedView(eventData.OriginalVictim);
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
            float targetRadius = (float)CommonReads.GetActorRadius(SimWorld, eventData.OriginalVictim);
            Vector2 victimImpactOffset = Vector2.ClampMagnitude((Vector2)eventData.ImpactVector, maxLength: targetRadius);
            displayedImpactPos = victimPos + victimImpactOffset;
        }

        displayedImpactPos += new Vector2(Random.Range(-NumberPositionRandomRadius, NumberPositionRandomRadius), Random.Range(-NumberPositionRandomRadius, NumberPositionRandomRadius));

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Request Number
        ////////////////////////////////////////////////////////////////////////////////////////
        {
            NumberSetting numberSetting;
            fix displayedValue = eventData.IsHeal ? eventData.TotalUncappedDelta : -eventData.TotalUncappedDelta;
            displayedValue = max(1, round(displayedValue));

            if (eventData.FinalVictim == Cache.PlayerGroupEntity)
            {
                numberSetting = eventData.IsHeal ? SettingGroupHealthGain : SettingGroupHealthLoss;
            }
            else
            {
                if (eventData.FirstInstigatorActor == PlayerHelpers.GetLocalSimPawnEntity(SimWorld))
                {
                    numberSetting = eventData.IsAutoAttack ? SettingLocalAA : SettingLocalSpell;
                }
                else
                {
                    numberSetting = eventData.IsAutoAttack ? SettingAllyAA : SettingAllySpell;
                }
            }

            if (numberSetting != null && numberSetting.Scale != 0 && numberSetting.Color.a > 0)
                GameSystem<FloatingTextSystem>.Instance.RequestText(displayedImpactPos, numberSetting.Scale * Vector2.one, displayedValue.ToString(), numberSetting.Color);
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