using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public class SimInputKeycode : SimPlayerInput
{
    public enum State
    {
        Pressed,
        Held,
        Released
    }

    public State state = State.Pressed;
    public KeyCode keyCode;
}
