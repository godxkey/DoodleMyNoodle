using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreServiceManagerBootstrap : MonoBehaviour
{
    void Awake()
    {
        if(CoreServiceManager.Instance == null)
        {
            new CoreServiceManager();
        }
    }
}
