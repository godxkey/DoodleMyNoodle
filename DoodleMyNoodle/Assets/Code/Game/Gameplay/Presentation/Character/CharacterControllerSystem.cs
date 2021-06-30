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

        // TODO : remove when conflict between boots speed and character speed is figured out
        fix speed = (fix)0.05;

        if (Input.GetKey(KeyCode.D))
        {
            SimPlayerInputMovingCharacter simInput = new SimPlayerInputMovingCharacter(new fix2(speed, 0));
            SimWorld.SubmitInput(simInput);
        }

        if (Input.GetKey(KeyCode.A))
        {
            SimPlayerInputMovingCharacter simInput = new SimPlayerInputMovingCharacter(new fix2(-speed, 0));
            SimWorld.SubmitInput(simInput);
        }
    }
}