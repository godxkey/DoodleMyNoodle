using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingScreenUIController : MonoBehaviour
{
    public static string DisplayedStatus;

    [SerializeField] TextMeshProUGUI _text;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void StaticReset()
    {
        DisplayedStatus = null;
    }

    void Update()
    {
        _text.text = DisplayedStatus;
    }
}
