using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Collections;
using Unity.Entities;

public class PawnSpriteColor : GamePresentationBehaviour
{
    public SpriteRenderer SpriteRenderer;

    public bool IsElite = false;

    protected override void OnGamePresentationUpdate()
    {
        if (TryGetComponent(out BindedSimEntityManaged bindedSimEntity))
        {
            Entity pawnController = CommonReads.TryGetPawnController(SimWorld, bindedSimEntity.SimEntity);
            if (SimWorld.TryGetComponent(pawnController, out Team pawnTeam))
            {
                Color spriteColor = new Color();
                switch ((DesignerFriendlyTeam)pawnTeam.Value)
                {
                    case DesignerFriendlyTeam.Player:
                        spriteColor = Color.white;
                        break;

                    case DesignerFriendlyTeam.Baddies:
                        if (IsElite)
                        {
                            spriteColor = Color.magenta;
                        }
                        else
                        {
                            spriteColor = Color.red;
                        }
                        break;
                }

                SpriteRenderer.color = spriteColor;
                return;
            }
        }
    }
}
