using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class MoveEnergyDisplay : GameMonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup = null;
    [SerializeField] private float _canvasFadeSpeed = 0.1f;

    [SerializeField] private float HideDelayFromMouse = 0.4f;
    [SerializeField] private float HideDelayFromMovement = 1f;

    [SerializeField] private Slider EnergyBar = null;

    private float _fadeDelayTimer;

    public void SetMoveEnergy(float value, float maxValue)
    {
        if (value != EnergyBar.value)
            ShowFromMovement();

        EnergyBar.maxValue = maxValue;
        EnergyBar.value = value;
    }

    private void Update()
    {
        if (EnergyBar.value == 0)
        {
            _fadeDelayTimer -= Time.deltaTime;

            if (_fadeDelayTimer <= 0)
            {
                _canvasGroup.alpha -= (Time.deltaTime * _canvasFadeSpeed);
            }
        }
    }

    public void ShowFromMouse()
    {
        if (EnergyBar.value > 0)
        {
            _canvasGroup.alpha = 1;
            _fadeDelayTimer = Mathf.Max(HideDelayFromMouse, _fadeDelayTimer);
        }
    }

    public void ShowFromMovement()
    {
        if (EnergyBar.value > 0)
        {
            _canvasGroup.alpha = 1;
            _fadeDelayTimer = Mathf.Max(HideDelayFromMovement, _fadeDelayTimer);
        }
    }
}