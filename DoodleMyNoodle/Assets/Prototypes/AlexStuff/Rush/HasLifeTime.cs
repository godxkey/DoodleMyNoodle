using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasLifeTime : MonoBehaviour
{
    public int duration = 1;

    void Start()
    {
        this.DelayedCall(duration, delegate ()
        {
            Destroy(gameObject);
        });
    }
}
