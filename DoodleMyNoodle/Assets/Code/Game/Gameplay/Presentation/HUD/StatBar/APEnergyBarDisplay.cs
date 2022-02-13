using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class APEnergyBarDisplay : StatBarDisplay
{
    [SerializeField] private Slider PreviewEnergyBar = null;
    [SerializeField] private Image EnergyBarSprite = null;
    [SerializeField] private float PreviewFadeDuration = 1;

    private Sequence _sq;
    private Color _startColor;

    protected override float GetStatBarMaxValue() { return (float)Cache.PlayerMaxAP; }
    protected override float GetStatBarValue() { return (float)Cache.PlayerAP; }

    public override void OnGameAwake() 
    {
        _startColor = EnergyBarSprite.color;
    }

    public override void SetStatBar(float value, float maxValue)
    {
        base.SetStatBar(value, maxValue);

        PreviewEnergyBar.maxValue = maxValue;
    }

    public void ShowPrevewAPEnergyCost(float value)
    {
        if (PreviewEnergyBar == null)
            return;

        StopShowingPreview();

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
}
