using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public static class GameActionPresentationFeedbacks
{
    // TODO : Remove This (Replace animation system by event base)
    public static void OnGameActionUsed(ISimWorldReadWriteAccessor accessor, GameAction.UseContext context, GameAction.ResultData result)
    {
        // ANIMATIONS
        List<KeyValuePair<string, object>> animationData = new List<KeyValuePair<string, object>>(result.Data); // to be passed in params[]
        animationData.Add(new KeyValuePair<string, object>("GameActionEntity", context.Item)); // animation need some contextual data
        CommonWrites.SetEntityAnimation(accessor, context.InstigatorPawn, animationData.ToArray());
    }
}