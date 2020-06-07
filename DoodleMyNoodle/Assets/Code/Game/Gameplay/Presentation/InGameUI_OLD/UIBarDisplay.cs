using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UIBarDisplay : GameMonoBehaviour
{
    public GameObject ValueBar;

    public void AjustDisplay(float ratio)
    {
        ValueBar.transform.localScale = new Vector3(Mathf.Lerp(0, 1, ratio), 1, 1);
    }
}
