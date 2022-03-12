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
        public int MaxTargetHit = 1;
        [Header("Default Parameter")]
        public Vector2 AttackVector = Vector2.right * 2;
        public ActorFilter TargetFilter = ActorFilter.Enemies;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Range = Range,
                Damage = Damage,
                ImpulseUpAngleRatio = ImpulseUpAngleRatio,
                ImpulseForce = ImpulseForce,
                MaxTargetHit = MaxTargetHit,
                AttackVector = (fix2)AttackVector,
                TargetFilter = TargetFilter,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix Range;
        public int Damage;
        public fix ImpulseUpAngleRatio;
        public fix ImpulseForce;
        public int MaxTargetHit;
        public fix2 AttackVector;
        public ActorFilter TargetFilter;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        GameActionParameterPosition.Description tileParam = new GameActionParameterPosition.Description()
        {
            MaxRangeFromInstigator = settings.Range
        };

        GameActionParameterSuccessRate.Description successParam = new GameActionParameterSuccessRate.Description();

        return new ExecutionContract(tileParam, successParam);
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        fix2 instigatorPos = input.Accessor.GetComponent<FixTranslation>(input.Context.LastPhysicalInstigator);

        fix2 position;
        if (input.Parameters != null && input.Parameters.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        {
            position = paramPosition.Position;
        }
        else
        {
            position = instigatorPos + settings.AttackVector;
        }

        // make sure survey position is within range
        fix2 attackPosition = Helpers.ClampPositionInsideRange(position, instigatorPos, settings.Range);

        // find all targets hit
        NativeList<Entity> hitTargets = new NativeList<Entity>(Allocator.Temp);

        var instigatorFilterInfo = CommonReads.GetActorFilterInfo(input.Accessor, input.Context.ActionInstigatorActor);

        var rayHits = CommonReads.Physics.CastRay(input.Accessor, instigatorPos, attackPosition, ignoreEntity: input.Context.FirstPhysicalInstigator);
        for (int i = 0; i < rayHits.Length; i++)
        {
            if (hitTargets.Length >= settings.MaxTargetHit)
                break;

            var targetFilterInfo = CommonReads.GetActorFilterInfo(input.Accessor, rayHits[i].Entity);

            if (Helpers.ActorFilterMatches(instigatorFilterInfo, targetFilterInfo, settings.TargetFilter))
            {
                hitTargets.AddUnique(rayHits[i].Entity);
            }
        }

        // get success multipliers
        int damage = settings.Damage;
        fix impulseMultipler = 1;
        if (input.Parameters != null && input.Parameters.TryGetParameter(1, out GameActionParameterSuccessRate.Data successRate, warnIfFailed: false))
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
                if (input.Accessor.TryGetComponent(hit, out FixTranslation translation) && input.Accessor.HasComponent<TileColliderTag>(hit))
                {
                    int2 pos = Helpers.GetTile(translation);
                    CommonWrites.RequestTransformTile(input.Accessor, pos, TileFlagComponent.Empty);
                }
                else
                {
                    CommonWrites.RequestImpulse(input.Accessor, hit, impulseVector);
                }
            }
        }

        // Apply damage
        CommonWrites.RequestDamage(input.Accessor, input.Context.LastPhysicalInstigator, hitTargets, damage);

        // Export action data used in event (animations use it)
        output.ResultData.Add(new ResultDataElement() { AttackVector = attackVector });

        return true;
    }
}
