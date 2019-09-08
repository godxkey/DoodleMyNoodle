using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCodeInputSender : MonoBehaviour
{
    // fbessette: we should make an editor window for that

    struct Entry
    {
        public KeyCode keyCode;
        public bool sendPress;
        public bool sendRelease;
        public bool sendHold;
    }

    Entry[] _enties = new Entry[]
    {
        ////////////////////////////////////////////////////////////////////////////////////////
        //      0 to 9
        ////////////////////////////////////////////////////////////////////////////////////////
        new Entry()
        {
            keyCode = KeyCode.Alpha1,
            sendPress = true
        }
        , new Entry()
        {
            keyCode = KeyCode.Alpha2,
            sendPress = true
        }
        , new Entry()
        {
            keyCode = KeyCode.Alpha3,
            sendPress = true
        }
        , new Entry()
        {
            keyCode = KeyCode.Alpha4,
            sendPress = true
        }
        , new Entry()
        {
            keyCode = KeyCode.Alpha5,
            sendPress = true
        }
        , new Entry()
        {
            keyCode = KeyCode.Alpha6,
            sendPress = true
        }
        , new Entry()
        {
            keyCode = KeyCode.Alpha7,
            sendPress = true
        }
        , new Entry()
        {
            keyCode = KeyCode.Alpha8,
            sendPress = true
        }
        , new Entry()
        {
            keyCode = KeyCode.Alpha9,
            sendPress = true
        }
        , new Entry()
        {
            keyCode = KeyCode.Alpha0,
            sendPress = true
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////
        //      ARROWS
        ////////////////////////////////////////////////////////////////////////////////////////
        , new Entry()
        {
            keyCode = KeyCode.RightArrow,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.LeftArrow,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.UpArrow,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.DownArrow,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////
        //      Letters
        ////////////////////////////////////////////////////////////////////////////////////////
        , new Entry()
        {
            keyCode = KeyCode.Q,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.W,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.E,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.R,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }
        , new Entry()
        {
            keyCode = KeyCode.T,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.Y,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.U,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.I,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.O,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.P,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.A,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.S,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.D,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.F,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.G,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.H,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.J,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.K,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.L,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.Z,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.X,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.C,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.V,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.B,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.N,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }, new Entry()
        {
            keyCode = KeyCode.M,
            sendPress = true,
            sendHold = true,
            sendRelease = true,
        }
    };


    void Update()
    {

        for (int i = 0; i < _enties.Length; i++)
        {
            if (_enties[i].sendPress && Input.GetKeyDown(_enties[i].keyCode))
            {
                Send(_enties[i].keyCode, SimInputKeycode.State.Pressed);
            }
            if (_enties[i].sendHold && Input.GetKey(_enties[i].keyCode))
            {
                Send(_enties[i].keyCode, SimInputKeycode.State.Held);
            }
            if (_enties[i].sendRelease && Input.GetKeyUp(_enties[i].keyCode))
            {
                Send(_enties[i].keyCode, SimInputKeycode.State.Released);
            }
        }
    }

    void Send(KeyCode keyCode, SimInputKeycode.State state)
    {
        SimulationController.Instance.SubmitInput(new SimInputKeycode() { keyCode = keyCode, state = state });
    }
}
