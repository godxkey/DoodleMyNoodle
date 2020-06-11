using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingScreenUIController : MonoBehaviour
{
    public static string displayedStatus;

    [SerializeField] TextMeshProUGUI _text;

    void Update()
    {
        _text.text = displayedStatus;
    }
}
