using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimCompDemoBallMove : SimComponent, ISimPawnInputHandler, ISimTickable
{
    static readonly Fix64 speed = 6;

    FixVector3 inputDirection;

    public bool HandleInput(SimPlayerInput input)
    {
        if (input is SimInputKeycode inputKeycode)
        {
            switch (inputKeycode.keyCode)
            {
                case KeyCode.Space:
                {
                    if (inputKeycode.state == SimInputKeycode.State.Pressed)
                    {
                        FixVector3 dir = Simulation.Random.Direction3D();
                        SimTransform.LocalPosition += dir;
                    }
                    break;
                }

                case KeyCode.RightArrow:
                {
                    if (inputKeycode.state == SimInputKeycode.State.Pressed)
                        inputDirection += FixVector3.right;
                    else if (inputKeycode.state == SimInputKeycode.State.Released)
                        inputDirection -= FixVector3.right;
                    break;
                }

                case KeyCode.LeftArrow:
                {
                    if (inputKeycode.state == SimInputKeycode.State.Pressed)
                        inputDirection += FixVector3.left;
                    else if (inputKeycode.state == SimInputKeycode.State.Released)
                        inputDirection -= FixVector3.left;
                    break;
                }

                case KeyCode.UpArrow:
                {
                    if (inputKeycode.state == SimInputKeycode.State.Pressed)
                        inputDirection += FixVector3.forward;
                    else if (inputKeycode.state == SimInputKeycode.State.Released)
                        inputDirection -= FixVector3.forward;
                    break;
                }

                case KeyCode.DownArrow:
                {
                    if (inputKeycode.state == SimInputKeycode.State.Pressed)
                        inputDirection += FixVector3.backward;
                    else if (inputKeycode.state == SimInputKeycode.State.Released)
                        inputDirection -= FixVector3.backward;
                    break;
                }
            }

            return true;
        }

        return false;
    }

    public void OnSimTick()
    {
        SimTransform.LocalPosition += inputDirection * speed * Simulation.DeltaTime;
    }
}
