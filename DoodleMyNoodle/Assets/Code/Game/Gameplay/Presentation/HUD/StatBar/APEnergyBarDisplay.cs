using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class APEnergyBarDisplay : StatBarDisplay
{
    [SerializeField] private Slider _previewEnergyBar = null;
    [SerializeField] private Image _energyBarSprite = null;
    [SerializeField] private float _previewFadeDuration = 1;

    private Sequence _sq;
    private Color _startColor;

    protected override float GetStatBarMaxValue() { return (float)Cache.PlayerMaxAP; }
    protected override float GetStatBarValue() { return (float)Cache.PlayerAP; }

    public override void OnGameAwake() 
    {
        _startColor = _energyBarSprite.color;
        _previewEnergyBar.gameObject.SetActive(false);
    }

    protected override void OnDestroy()
    {
        _sq?.KillIfActive();
        base.OnDestroy();
    }

    public override void SetStatBar(float value, float maxValue)
    {
        base.SetStatBar(value, maxValue);

        _previewEnergyBar.maxValue = maxValue;
    }

    public void ShowPrevewAPEnergyCost(float value)
    {
        if (_previewEnergyBar == null)
            return;

        StopShowingPreview();

        _previewEnergyBar.value = value;
        _energyBarSprite.color = Color.red;
        _previewEnergyBar.gameObject.SetActive(true);

        _sq = DOTween.Sequence();
        _sq.Join(_energyBarSprite.DOFade(0, _previewFadeDuration/2));
        _sq.Append(_energyBarSprite.DOFade(1, _previewFadeDuration/2));
        _sq.SetLoops(-1, LoopType.Yoyo);
    }

    public void StopShowingPreview()
    {
        _energyBarSprite.color = _startColor;
        _energyBarSprite.SetAlpha(1);
        _sq.Kill();
        _previewEnergyBar.gameObject.SetActive(false);
    }
}
