using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Assertions;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textDisplay;
    [SerializeField] float _sampleDuration = 1;
    [SerializeField] int _decimalCount = 2;

    private float _cummulatedFPS = 0;
    private int _frameCounter = 0;
    private string _format;
    private float _nextSampleTime = 0;

    private static FPSCounter s_instance;

    private void Awake()
    {
        _format = "0.";
        for (int i = 0; i < _decimalCount; i++)
        {
            _format += "0";
        }

        Assert.IsNull(s_instance);
        s_instance = this;

        gameObject.SetActive(s_visibility);
    }

    private void OnDestroy()
    {
        s_instance = null;
    }

    void Update()
    {
        _frameCounter++;
        _cummulatedFPS += FPSHelper.CurrentFPS;

        if (Time.time > _nextSampleTime)
        {
            UpdateDisplay();
            _nextSampleTime = Time.time + _sampleDuration;
            _frameCounter = 0;
            _cummulatedFPS = 0;
        }
    }

    private void UpdateDisplay()
    {
        _textDisplay.text = (_cummulatedFPS / _frameCounter).ToString(_format);
    }

    private static bool s_visibility;

    [ConsoleVar("FPSCount", "The visibility of the FPS counter module.", Save = ConsoleVarAttribute.SaveMode.PlayerPrefs)]
    public static bool Visibility
    {
        get => s_visibility;
        set
        {
            s_visibility = value;
            s_instance?.gameObject.SetActive(value);
        }
    }
}
