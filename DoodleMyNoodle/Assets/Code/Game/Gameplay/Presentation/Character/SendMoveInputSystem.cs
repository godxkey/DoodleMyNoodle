using CCC.Fix2D;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class SendMoveInputSystem : GamePresentationSystem<SendMoveInputSystem>
{
    private DirtyValue<fix2> _moveInput;

    protected override void OnGamePresentationUpdate()
    {
        if (!Cache.LocalPawnExists)
            return;

        if (SimWorld.TryGetComponent(Cache.LocalPawn, out CanMoveFreely canMoveFreely)) 
        {
            if (!canMoveFreely.CanMove)
                return;
        }
        else
        {
            return;
        }

        HandleDirectionalMove();
        HandleJump();
    }

    private void HandleDirectionalMove()
    {
        fix2 input = fix2.zero;

        if (UIStateMachineController.Instance.CurrentSate.Type == UIStateType.Gameplay)
        {
            if (Input.GetKey(KeyCode.D))
                input.x += 1;

            if (Input.GetKey(KeyCode.A))
                input.x += -1;

            if (Input.GetKey(KeyCode.W))
                input.y += 1;

            if (Input.GetKey(KeyCode.S))
                input.y += -1;
        }

        _moveInput.Set(input);

        // NB: we only send an input if the direction changes. That way, we minimize network messages
        if (_moveInput.ClearDirty())
        {
            SimPlayerInputMove simInput = new SimPlayerInputMove(_moveInput);
            SimWorld.SubmitInput(simInput);
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SimWorld.SubmitInput(new SimPlayerInputJump());
        }
    }
}