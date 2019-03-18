using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeycodeListener : MonoBehaviour
{
    public KeyCode keyCode;
    public UnityEvent onKeyDown = new UnityEvent();

    void Update()
    {
        if (Input.GetKeyDown(keyCode))
        {
            onKeyDown.Invoke();
        }
    }
}
