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
            // is an animation playing right ? aka do we have an animation data
            if (TryGetComponent(entity, out AnimationData animationData))
            {
                CommonReads.AnimationTypes currentPlayerAnimation = (CommonReads.AnimationTypes)animationState.StateID;

                // Animation Done, handle exit and transition
                if (Accessor.Time.ElapsedTime >= animationData.LastTransitionTime + animationData.TotalDuration)
                {
                    switch (currentPlayerAnimation)
                    {
                        // Example of a looping anim that has another condition to its end
                        case CommonReads.AnimationTypes.Walking:
                            if (!EntityManager.HasComponent<PathPosition>(entity))
                            {
                                CommonWrites.SetEntityAnimation(Accessor, entity, CommonReads.AnimationTypes.Idle);
                            }
                            break;
                            
                        // Example of a one time anim that ends after it's done
                        case CommonReads.AnimationTypes.GameAction:
                            CommonWrites.SetEntityAnimation(Accessor, entity, CommonReads.AnimationTypes.Idle);
                            break;
                    }
                }
            }
            else
            {
                // By Default, when game starts we need to enter IDLE State
                if (animationState.StateID == (int)CommonReads.AnimationTypes.None)
                {
                    CommonWrites.SetEntityAnimation(Accessor, entity, CommonReads.AnimationTypes.Idle);
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
        GameAction
    }
}

internal static partial class CommonWrites
{
    public static void SetEntityAnimation(ISimWorldReadWriteAccessor accessor, Entity entity, CommonReads.AnimationTypes animationType, params KeyValuePair<string, object>[] additionnalData)
    {
        accessor.SetOrAddComponentData(entity, GetAnimationData(accessor, animationType, additionnalData));
        accessor.SetOrAddComponentData(entity, new AnimationState() { StateID = (int)animationType });
    }

    private static AnimationData GetAnimationData(ISimWorldReadWriteAccessor accessor, CommonReads.AnimationTypes animationType, params KeyValuePair<string, object>[] additionnalData)
    {
        AnimationData newAnimationData = new AnimationData();
        newAnimationData.Direction = GetAdditionnalAnimationData<int2>("Direction", additionnalData);
        newAnimationData.GameActionEntity = GetAdditionnalAnimationData<Entity>("GameActionEntity", additionnalData);
        newAnimationData.LastTransitionTime = accessor.Time.ElapsedTime;

        // Data defined by animation hard coded here, could be filled elsewhere the system has access to
        switch (animationType)
        {
            case CommonReads.AnimationTypes.Idle:
                newAnimationData.TotalDuration = (fix)0.75f;
                break;
            case CommonReads.AnimationTypes.Walking:
                newAnimationData.TotalDuration = (fix)0.25f;
                break;
            case CommonReads.AnimationTypes.GameAction:
                newAnimationData.TotalDuration = (fix)0.5f;
                break;
            default:
                break;
        }

        return newAnimationData;
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