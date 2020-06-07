using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Collections;
using Unity.Entities;

public class PawnSpriteColor : GameMonoBehaviour
{
    public SpriteRenderer SpriteRenderer;

    public bool IsElite = false;

    public override void OnGameLateUpdate()
    {
        int2 currentTilePos = new int2((int)transform.position.x,(int)transform.position.y);
        ExternalSimWorldAccessor accessor = GameMonoBehaviourHelpers.GetSimulationWorld();

        NativeList<Entity> pawnsOnTile = new NativeList<Entity>(Allocator.Temp);
        CommonReads.FindEntitiesOnTileWithComponent<ControllableTag>(accessor, currentTilePos, pawnsOnTile);
        foreach (Entity pawn in pawnsOnTile)
        {
            Entity pawnController = CommonReads.GetPawnController(accessor, pawn);
            if (accessor.TryGetComponentData(pawnController, out Team pawnTeam))
            {
                Color spriteColor = new Color();
                switch ((TeamAuth.DesignerFriendlyTeam)pawnTeam.Value)
                {
                    case TeamAuth.DesignerFriendlyTeam.Player:
                        spriteColor = Color.white;
                        break;
                    case TeamAuth.DesignerFriendlyTeam.Baddies:
                        if (IsElite)
                        {
                            spriteColor = Color.magenta;
                        }
                        else
                        {
                            spriteColor = Color.red;
                        }
                        break;
                    default:
                        break;
                }

                SpriteRenderer.color = spriteColor;
                return;
            }
        }
    }
}
