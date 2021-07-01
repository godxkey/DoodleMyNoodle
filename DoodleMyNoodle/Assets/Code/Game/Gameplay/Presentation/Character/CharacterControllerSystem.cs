using CCC.Fix2D;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class CharacterControllerSystem : GamePresentationSystem<CharacterControllerSystem>
{
    private bool _isJumping = false;

    protected override void OnGamePresentationUpdate()
    {
        if ((UIStateMachineController.Instance.CurrentSate.Type != UIStateType.Gameplay))
            return;

        fix horizontalMovement = 0;
        fix verticalMovement = 0;

        if (Input.GetKey(KeyCode.D))
        {
            horizontalMovement = 1;
        } 
        else if (Input.GetKey(KeyCode.A))
        {
            horizontalMovement = -1;
        }

        if (Input.GetKey(KeyCode.W))
        {
            verticalMovement = 1;
        } 
        else if (Input.GetKey(KeyCode.S))
        {
            verticalMovement = -1;
        }

        SimPlayerInputMovingCharacter simInput = new SimPlayerInputMovingCharacter(new fix2(horizontalMovement, verticalMovement));
        SimWorld.SubmitInput(simInput);

        if (Input.GetKey(KeyCode.Space) && !_isJumping && SimWorld.GetComponent<NavAgentFootingState>(Cache.LocalPawn).Value == NavAgentFooting.Ground)
        {
            _isJumping = true;

            if (SimWorld.TryGetBufferReadOnly(Cache.LocalPawn, out DynamicBuffer<InventoryItemReference> inventory))
            {
                InventoryItemReference item = inventory[0];

                if (SimWorld.Exists(item.ItemEntity))
                {
                    Entity itemEntity = item.ItemEntity;

                    UIStateMachine.Instance.TransitionTo(UIStateType.ParameterSelection, new ParameterSelectionState.InputParam()
                    {
                        ObjectEntity = itemEntity,
                        IsItem = true,
                        ItemIndex = 0
                    });
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space) && _isJumping)
        {
            _isJumping = false;
        }
    }
}