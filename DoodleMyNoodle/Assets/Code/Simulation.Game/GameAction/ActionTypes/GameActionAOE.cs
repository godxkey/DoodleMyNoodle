using Unity.Entities;
using System;
using System.Collections.Generic;
using CCC.Fix2D;
using CCC.InspectorDisplay;
using UnityEngine;
using Unity.Collections;

public class GameActionAOE : GameAction<GameActionAOE.Settings>
{
    public enum Shape
    {
        Circle,
        Box
    }

    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public ActorFilter ActorFilter;
        public Shape Shape;

        [ShowIf(nameof(IsBox))]
        public Vector2 BoxSize = Vector2.one;

        [ShowIf(nameof(IsCircle))]
        public float CircleRadius = 1;

        public Vector2 InstigatorOffset;

        public List<GameObject> Actions = new List<GameObject>();

        private bool IsBox => Shape == Shape.Box;
        private bool IsCircle => Shape == Shape.Circle;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                ActorFilter = ActorFilter,
                Shape = Shape,
                Dimensions = Shape switch
                {
                    Shape.Box => (fix2)BoxSize,
                    Shape.Circle => new fix2((fix)CircleRadius),
                    _ => throw new NotImplementedException()
                },
                InstigatorOffset = (fix2)InstigatorOffset,
            });

            var actions = dstManager.AddBuffer<SettingsAction>(entity);
            foreach (var item in Actions)
            {
                if (item != null)
                    actions.Add(conversionSystem.GetPrimaryEntity(item));
            }
        }

        public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            base.DeclareReferencedPrefabs(referencedPrefabs);

            foreach (var item in Actions)
            {
                if (item != null)
                {
                    referencedPrefabs.Add(item);
                }
            }
        }
    }

    public struct Settings : IComponentData
    {
        public Shape Shape;
        public fix2 InstigatorOffset;
        public fix2 Dimensions;
        public ActorFilter ActorFilter;
    }

    public struct SettingsAction : IBufferElementData
    {
        public Entity Value;

        public static implicit operator Entity(SettingsAction val) => val.Value;
        public static implicit operator SettingsAction(Entity val) => new SettingsAction() { Value = val };
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract();
        // n.b. maybe entity or position in the future ?
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        fix2 lastInstigPos = input.Accessor.GetComponent<FixTranslation>(input.ActionInstigatorActor);
        fix2 aoeCenter = lastInstigPos + settings.InstigatorOffset;
        ActorFilterInfo instigatorFilterInfo = CommonReads.GetActorFilterInfo(input.Accessor, input.ActionInstigatorActor);

        NativeList<Entity> newTargets = new NativeList<Entity>(Allocator.Temp);

        switch (settings.Shape)
        {
            case Shape.Circle:
            {
                NativeList<DistanceHit> hits = CommonReads.Physics.OverlapCircle(input.Accessor, aoeCenter, settings.Dimensions.x);
                hits.CopyToEntityList(newTargets);
                break;
            }

            case Shape.Box:
            {
                fix2 halfSize = settings.Dimensions / 2;
                NativeList<Entity> hits = CommonReads.Physics.OverlapAabb(input.Accessor, min: aoeCenter - halfSize, aoeCenter + halfSize);
                newTargets.AddRange(hits);
                break;
            }
            default:
                throw new NotImplementedException();
        }

        CommonReads.FilterActors(input.Accessor, newTargets, instigatorFilterInfo, settings.ActorFilter);

        var gameActionSystem = input.Accessor.GetExistingSystem<ExecuteGameActionSystem>();
        var actions = input.Accessor.GetBuffer<SettingsAction>(input.Context.Action);
        foreach (var action in actions)
        {
            gameActionSystem.ActionRequestsManaged.Add(new GameActionRequestManaged()
            {
                Instigator = input.Context.ActionInstigator,
                ActionEntity = action,
                Targets = newTargets
            });
        }

        return true;
    }
}