using CCC.Fix2D;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class SendMoveInputSystem : GamePresentationSystem<SendMoveInputSystem>
{
    private bool _muteVerticalInputUntilRelease;
    private DirtyValue<fix2> _moveInput;

    public override void PresentationUpdate()
    {
        if (!Cache.LocalPawnExists)
            return;

        if (GameConsole.IsOpen())
            return;

        // This should be done BEFORE directional move because Jumping can inhibit the 'up' move
        HandleJump();

        HandleDirectionalMove();
    }

    private void HandleDirectionalMove()
    {
        fix2 input = fix2.zero;

        bool up = Input.GetKey(KeyCode.W);
        bool down = Input.GetKey(KeyCode.S);
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);
        
        if (_muteVerticalInputUntilRelease && !up && !down)
        {
            _muteVerticalInputUntilRelease = false;
        }

        if (UIStateMachineController.Instance.CurrentSate.Type == UIStateType.Gameplay)
        {
            if (right)
                input.x += 1;

            if (left)
                input.x += -1;

            if (up && !_muteVerticalInputUntilRelease)
                input.y += 1;

            if (down && !_muteVerticalInputUntilRelease)
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

            // This will mute all 'up' input until the player releases the 'up' key.
            // This is used to prevent the player for accidentally grabbing onto ladders when pressing both W and jump together
            _muteVerticalInputUntilRelease = true;
        }
    }
}