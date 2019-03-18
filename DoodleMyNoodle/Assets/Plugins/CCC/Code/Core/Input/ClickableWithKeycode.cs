using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickableWithKeycode : MonoBehaviour
{
    public KeyCode keyCode;

    [SerializeField] Button _button;

    void Update()
    {
        if (Input.GetKeyDown(keyCode) && _button.interactable)
        {
            _button.onClick.Invoke();
        }
    }
}
