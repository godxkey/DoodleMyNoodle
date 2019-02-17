using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleClientServerController : MonoBehaviour
{
    public JobifiedClientBehaviour clientBehaviour;
    public JobifiedServerBehaviour serverBehaviour;

    void Awake()
    {
        var commandLineArgs = new List<string>(Environment.GetCommandLineArgs());

        bool isHeadless = commandLineArgs.Contains("-batchmode");

        clientBehaviour.gameObject.SetActive(!isHeadless);
        serverBehaviour.gameObject.SetActive(isHeadless);
    }
}
