using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimCompDemoBallMove : SimComponent, ISimPawnInputHandler, ISimTickable
{
    static readonly Fix64 speed = 6;

    FixVector3 inputDirection;

    public bool HandleInput(SimInput input)
    {
        if (input is SimInputKeycode inputKeycode)
        {
            switch (inputKeycode.keyCode)
            {
                case KeyCode.Space:
                {
                    FixVector3 dir = Simulation.Random.Direction3D();
                    SimTransform.localPosition += dir;
                    break;
                }

                case KeyCode.RightArrow:
                {
                    if (inputKeycode.state == SimInputKeycode.State.Pressed)
                        inputDirection += FixVector3.right;
                    else
                        inputDirection -= FixVector3.right;
                    break;
                }

                case KeyCode.LeftArrow:
                {
                    if (inputKeycode.state == SimInputKeycode.State.Pressed)
                        inputDirection += FixVector3.left;
                    else
                        inputDirection -= FixVector3.left;
                    break;
                }

                case KeyCode.UpArrow:
                {
                    if (inputKeycode.state == SimInputKeycode.State.Pressed)
                        inputDirection += FixVector3.forward;
                    else
                        inputDirection -= FixVector3.forward;
                    break;
                }

                case KeyCode.DownArrow:
                {
                    if (inputKeycode.state == SimInputKeycode.State.Pressed)
                        inputDirection += FixVector3.backward;
                    else
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
        SimTransform.localPosition += inputDirection * speed * Simulation.DeltaTime;
    }
}
