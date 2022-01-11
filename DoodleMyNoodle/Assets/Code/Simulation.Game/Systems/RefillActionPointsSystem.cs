using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(PreAISystemGroup))]
public class RefillActionPointsSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        if (HasSingleton<NewTurnEventData>())
        {
            NativeList<Entity> playingEntities = new NativeList<Entity>(Allocator.Temp);
            CommonReads.GetCurrentlyPlayingEntities(Accessor, playingEntities);

            foreach (var controller in playingEntities)
            {
                if (TryGetComponent(controller, out ControlledEntity pawn))
                {
                    if (HasComponent<MaximumFix<ActionPoints>>(pawn) &&
                        HasComponent<ActionPoints>(pawn))
                    {
                        SetComponent<ActionPoints>(pawn, GetComponent<MaximumFix<ActionPoints>>(pawn).Value);
                    }
                }
            }
        }
    }
}
