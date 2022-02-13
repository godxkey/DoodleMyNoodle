using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using System.Collections.Generic;
using UnityEngine;
using CCC.Fix2D;
using System;
using Unity.MathematicsX;
using Unity.Entities;
using Unity.Collections;
using CCC.InspectorDisplay;

public class GameActionMeleeAttack : GameAction<GameActionMeleeAttack.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Range = 1;
        public int Damage = 1;
        public fix ImpulseForce = 0;
        public fix ImpulseUpAngleRatio = 1;
        public bool HitTargetsInbetween = false;
        [Header("Default Parameter")]
        public Vector2 DisplacementVectorToAttackPosition;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Range = Range,
                Damage = Damage,
                ImpulseUpAngleRatio = ImpulseUpAngleRatio,
                ImpulseForce = ImpulseForce,
                HitTargetsInbetween = HitTargetsInbetween,
                DisplacementVectorToAttackPosition = new fix2((fix)DisplacementVectorToAttackPosition.x, (fix)DisplacementVectorToAttackPosition.y)
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix Range;
        public int Damage;
        public fix ImpulseUpAngleRatio;
        public fix ImpulseForce;
        public bool HitTargetsInbetween;
        public fix2 DisplacementVectorToAttackPosition;
    }

    public override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, Settings settings)
    {
        GameActionParameterPosition.Description tileParam = new GameActionParameterPosition.Description()
        {
            MaxRangeFromInstigator = settings.Range
        };

        GameActionParameterSuccessRate.Description successParam = new GameActionParameterSuccessRate.Description();

        return new ExecutionContract(tileParam, successParam);
    }

    public override bool Use(ISimGameWorldReadWriteAccessor accessor, in ExecutionContext context, UseParameters useData, List<ResultDataElement> resultData, Settings settings)
    {
        fix2 instigatorPos = accessor.GetComponent<FixTranslation>(context.FirstInstigatorActor);

        fix2 position;
        if (useData != null && useData.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        {
            position = paramPosition.Position;
        }
        else
        {
            position = instigatorPos + settings.DisplacementVectorToAttackPosition;
        }

        // make sure survey position is within range
        fix2 attackPosition = Helpers.ClampPositionInsideRange(position, instigatorPos, settings.Range);
        fix attackRadius = (fix)0.1f;

        // find all targets hit
        NativeList<Entity> hitTargets = new NativeList<Entity>(Allocator.Temp);
        NativeList<DistanceHit> overlapHits = CommonReads.Physics.OverlapCircle(accessor, attackPosition, attackRadius, ignoreEntity: context.FirstInstigatorActor);

        foreach (var hit in overlapHits)
        {
            hitTargets.Add(hit.Entity);
        }

        if (settings.HitTargetsInbetween)
        {
            var rayHits = CommonReads.Physics.CastRay(accessor, instigatorPos, attackPosition, ignoreEntity: context.FirstInstigatorActor);
            for (int i = 0; i < rayHits.Length; i++)
            {
                hitTargets.AddUnique(rayHits[i].Entity);
            }
        }

        // get success multipliers
        int damage = settings.Damage;
        fix impulseMultipler = 1;
        if (useData != null && useData.TryGetParameter(1, out GameActionParameterSuccessRate.Data successRate, warnIfFailed: false))
        {
            switch (successRate.SuccessRate)
            {
                case SurveySuccessRating.One:
                    impulseMultipler = (fix)0.75f;
                    break;
                case SurveySuccessRating.Two:
                    damage += 1;
                    break;
                case SurveySuccessRating.Three:
                    damage += 1;
                    break;
                case SurveySuccessRating.Four:
                    damage += 1;
                    break;
                case SurveySuccessRating.Five:
                    impulseMultipler = (fix)1.5;
                    damage *= 2;
                    break;
                default:
                    break;
            }
        }

        fix2 attackVector = attackPosition - instigatorPos;

        // Apply impulse (if any)
        fix impulseMagnitude = impulseMultipler * settings.ImpulseForce;
        if (impulseMagnitude > 0)
        {
            fix settingsAngle = settings.ImpulseUpAngleRatio * Angle2DUp;
            fix impulseAngle = attackVector.x > 0 ? settingsAngle : Angle2DLeft - settingsAngle;
            fix2 impulseVector = fix2.FromAngle(impulseAngle) * impulseMagnitude;

            foreach (var hit in hitTargets)
            {
                if (accessor.TryGetComponent(hit, out FixTranslation translation) && accessor.HasComponent<TileColliderTag>(hit))
                {
                    int2 pos = Helpers.GetTile(translation);
                    CommonWrites.RequestTransformTile(accessor, pos, TileFlagComponent.Empty);
                }
                else
                {
                    CommonWrites.RequestImpulse(accessor, hit, impulseVector);
                }
            }
        }

        // Apply damage
        CommonWrites.RequestDamage(accessor, hitTargets, damage);

        // Export action data used in event (animations use it)
        resultData.Add(new ResultDataElement() { AttackVector = attackVector });

        return true;
    }
}