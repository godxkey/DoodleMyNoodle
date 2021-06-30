using CCC.Fix2D;
using System;
using UnityEngine;
using UnityEngineX;

public class CharacterControllerSystem : GamePresentationSystem<CharacterControllerSystem>
{
    protected override void OnGamePresentationUpdate()
    {
        if ((UIStateMachineController.Instance.CurrentSate.Type == UIStateType.ParameterSelection))
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
    }
}