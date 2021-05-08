using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class AnimationStateMachineSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref AnimationState animationState) =>
        {
            // is an animation playing or has been played ? aka do we have an animation data
            if (TryGetComponent(entity, out AnimationData animationData))
            {
                CommonReads.AnimationTypes currentPlayerAnimation = (CommonReads.AnimationTypes)animationState.StateID;

                // Animation Done, handle exit and transition
                if (Accessor.Time.ElapsedTime >= animationData.LastTransitionTime + animationData.TotalDuration)
                {
                    switch (currentPlayerAnimation)
                    {
                        // Example of an anim where we're stuck in the end state
                        case CommonReads.AnimationTypes.Death:
                            if (!EntityManager.HasComponent<DeadTag>(entity))
                            {
                                CommonWrites.SetEntityAnimation(Accessor, entity
                                    , new KeyValuePair<string, object>("Type", (int)CommonReads.AnimationTypes.Idle)
                                    , new KeyValuePair<string, object>("Duration", (fix)0.75f));
                            }
                            break;

                        // Example of a looping anim that has another condition to its end
                        case CommonReads.AnimationTypes.Walking:
                            if (!EntityManager.HasComponent<PathPosition>(entity))
                            {
                                CommonWrites.SetEntityAnimation(Accessor, entity
                                    , new KeyValuePair<string, object>("Type", (int)CommonReads.AnimationTypes.Idle)
                                    , new KeyValuePair<string, object>("Duration", (fix)0.75f));
                            }
                            break;
                            
                        // Example of a one time anim that ends after it's done
                        case CommonReads.AnimationTypes.GameAction:
                            CommonWrites.SetEntityAnimation(Accessor, entity
                                    , new KeyValuePair<string, object>("Type", (int)CommonReads.AnimationTypes.Idle)
                                    , new KeyValuePair<string, object>("Duration", (fix)0.75f));
                            break;
                    }
                }
            }
            else
            {
                // By Default, when game starts we need to enter IDLE State
                if (animationState.StateID == (int)CommonReads.AnimationTypes.None)
                {
                    CommonWrites.SetEntityAnimation(Accessor, entity
                                    , new KeyValuePair<string, object>("Type", (int)CommonReads.AnimationTypes.Idle)
                                    , new KeyValuePair<string, object>("Duration", (fix)0.75f));
                }
            }
        }).Run();
    }
}

public partial class CommonReads
{
    public enum AnimationTypes
    {
        None,
        Idle,
        Walking,
        Death,
        GameAction
    }
}

internal static partial class CommonWrites
{
    public static void SetEntityAnimation(ISimWorldReadWriteAccessor accessor, Entity entity, params KeyValuePair<string, object>[] additionnalData)
    {
        accessor.SetOrAddComponentData(entity, GetAnimationData(accessor, additionnalData));
        accessor.SetOrAddComponentData(entity, new AnimationState() { StateID = GetAnimationType(accessor,additionnalData) });
    }

    private static AnimationData GetAnimationData(ISimWorldReadWriteAccessor accessor, params KeyValuePair<string, object>[] additionnalData)
    {
        AnimationData newAnimationData = new AnimationData();
        newAnimationData.LastTransitionTime = accessor.Time.ElapsedTime;

        // Add things that can be useful
        newAnimationData.Direction = GetAdditionnalAnimationData<fix2>("Direction", additionnalData);
        newAnimationData.GameActionEntity = GetAdditionnalAnimationData<Entity>("GameActionEntity", additionnalData);

        // Get Duration for State Machine
        if (accessor.TryGetComponentData(newAnimationData.GameActionEntity, out GameActionSettingAnimationType animationTypeData))
        {
            newAnimationData.TotalDuration = animationTypeData.Duration;
        }
        else
        {
            // No component data and we're trying to do an anim ? maybe duration was manually feeded
            newAnimationData.TotalDuration = GetAdditionnalAnimationData<fix>("Duration", additionnalData);
        }

        return newAnimationData;
    }

    private static int GetAnimationType(ISimWorldReadWriteAccessor accessor, params KeyValuePair<string, object>[] additionnalData)
    {
        if (accessor.TryGetComponentData(GetAdditionnalAnimationData<Entity>("GameActionEntity", additionnalData), out GameActionSettingAnimationType animationTypeData))
        {
            return animationTypeData.AnimationType;
        }
        else
        {
            return GetAdditionnalAnimationData<int>("Type", additionnalData);
        }
    }

    private static T GetAdditionnalAnimationData<T>(string dataTypeID, params KeyValuePair<string, object>[] additionnalData)
    {
        foreach (KeyValuePair<string, object> data in additionnalData)
        {
            if (data.Key == dataTypeID)
            {
                return (T)data.Value;
            }
        }

        return default;
    }
}