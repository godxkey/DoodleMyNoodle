using System;
using TMPro;
using UnityEngine;
using UnityEngineX;

public class LifePointsDisplay : GamePresentationBehaviour
{
    [SerializeField] private TextMeshProUGUI _counterText;

    private DirtyValue<int> _lifePoints;

    protected override void OnGamePresentationUpdate()
    {
        _lifePoints.Set(Cache.GroupLifePoints);

        if (_lifePoints.ClearDirty())
        {
            _counterText.text = _lifePoints.Get().ToString();
        }
    }
}