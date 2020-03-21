using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimCompDemoBallMove : SimComponent, ISimPlayerInputHandler, ISimTickable
{
    static readonly Fix64 speed = 6;

    [System.Serializable]
    struct SerializedData
    {
        public FixVector3 InputDirection;
    }

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
                        _data.InputDirection += FixVector3.right;
                    else if (inputKeycode.state == SimInputKeycode.State.Released)
                        _data.InputDirection -= FixVector3.right;
                    break;
                }

                case KeyCode.LeftArrow:
                {
                    if (inputKeycode.state == SimInputKeycode.State.Pressed)
                        _data.InputDirection += FixVector3.left;
                    else if (inputKeycode.state == SimInputKeycode.State.Released)
                        _data.InputDirection -= FixVector3.left;
                    break;
                }

                case KeyCode.UpArrow:
                {
                    if (inputKeycode.state == SimInputKeycode.State.Pressed)
                        _data.InputDirection += FixVector3.forward;
                    else if (inputKeycode.state == SimInputKeycode.State.Released)
                        _data.InputDirection -= FixVector3.forward;
                    break;
                }

                case KeyCode.DownArrow:
                {
                    if (inputKeycode.state == SimInputKeycode.State.Pressed)
                        _data.InputDirection += FixVector3.backward;
                    else if (inputKeycode.state == SimInputKeycode.State.Released)
                        _data.InputDirection -= FixVector3.backward;
                    break;
                }
            }

            return true;
        }

        return false;
    }

    public void OnSimTick()
    {
        SimTransform.LocalPosition += _data.InputDirection * speed * Simulation.DeltaTime;
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [CCC.InspectorDisplay.AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        // define default values here
    };

    public override void PushToDataStack(SimComponentDataStack dataStack)
    {
        base.PushToDataStack(dataStack);
        dataStack.Push(_data);
    }

    public override void PopFromDataStack(SimComponentDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();
        base.PopFromDataStack(dataStack);
    }
    #endregion
}
