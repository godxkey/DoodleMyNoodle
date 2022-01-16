using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhySoManyErrors : MonoBehaviour
{
    public Text TextField;



    void Start()
    {
        Application.logMessageReceived += Application_logMessageReceived;
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            TextField.text += $"{condition}\n\n{stackTrace}\n\n";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
