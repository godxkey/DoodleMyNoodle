using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class APEnergyBarDisplay : GameMonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup = null;
    [SerializeField] private float _canvasFadeSpeed = 0.1f;

    [SerializeField] private float HideDelayFromMouse = 0.4f;
    [SerializeField] private float HideDelayFromMovement = 1f;

    [SerializeField] private Slider EnergyBar = null;
    [SerializeField] private Slider PreviewEnergyBar = null;
    [SerializeField] private Image EnergyBarSprite = null;
    [SerializeField] private float PreviewFadeDuration = 1;

    private Sequence _sq;
    private Color _startColor;

    private float _fadeDelayTimer;

    public override void OnGameAwake() 
    {
        _startColor = EnergyBarSprite.color;
    }

    public void SetAPEnergy(float value, float maxValue)
    {
        if (value != EnergyBar.value)
            ShowFromMovement();

        EnergyBar.maxValue = maxValue;
        EnergyBar.value = value;

        PreviewEnergyBar.maxValue = maxValue;
    }

    public void ShowPrevewAPEnergyCost(float value)
    {
        StopShowingPreview();

        if (PreviewEnergyBar == null)
            return;

        PreviewEnergyBar.value = value;
        EnergyBarSprite.color = Color.red;
        PreviewEnergyBar.gameObject.SetActive(true);

        _sq = DOTween.Sequence();
        _sq.Join(EnergyBarSprite.DOFade(0, PreviewFadeDuration/2));
        _sq.Append(EnergyBarSprite.DOFade(1, PreviewFadeDuration/2));
        _sq.SetLoops(-1, LoopType.Yoyo);
    }

    public void StopShowingPreview()
    {
        EnergyBarSprite.color = _startColor;
        EnergyBarSprite.SetAlpha(1);
        _sq.Kill();
        PreviewEnergyBar.gameObject.SetActive(false);
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