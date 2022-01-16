using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhySoManyErrors : MonoBehaviour
{
    public Text TextField;

    static public string ErrorText;

    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad() // Executed after scene is loaded and game is running
    {
        Application.logMessageReceived += Application_logMessageReceived;

        if (CoreServiceManager.instance == null)
        {
            new CoreServiceManager();
        }
    }

    void Update()
    {
        TextField.text = ErrorText;
    }

    static private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            ErrorText += $"{condition}\n\n{stackTrace}\n\n";
    }
}
