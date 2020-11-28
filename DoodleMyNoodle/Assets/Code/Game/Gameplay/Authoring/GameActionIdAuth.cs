using Unity.Entities;
using UnityEngine;
using System.Linq;
using System;
using CCC.InspectorDisplay;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class GameActionIdAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public string Value;
    public bool PlayAnimation = false;
    [ShowIf("PlayAnimation")]
    public AnimationDefinition Animation;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, GameActionBank.GetActionId(Value));
        if (Animation != null)
        {
            dstManager.AddComponentData(entity, new GameActionAnimationTypeData() { AnimationType = (int)Animation.AnimationType, Duration = (fix)Animation.Duration });
        }
    }
}