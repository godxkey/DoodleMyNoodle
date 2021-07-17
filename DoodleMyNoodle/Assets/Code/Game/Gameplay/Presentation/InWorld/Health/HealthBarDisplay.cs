using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;
using TMPro;

public class HealthBarDisplay : GameMonoBehaviour
{
    [FormerlySerializedAs("_emptyHearthPrefab")]
    [SerializeField] private GameObject _heartPrefab = null;
    [FormerlySerializedAs("_hearthContainer")]
    [SerializeField] private Transform _heartContainer = null;

    [FormerlySerializedAs("_emptyHearth")]
    [SerializeField] private Sprite _emptySprite = null;
    [FormerlySerializedAs("_filledHearth")]
    [SerializeField] private Sprite _filledSprite = null;

    [FormerlySerializedAs("_canvasGroup")]
    [SerializeField] private CanvasGroup _canvasGroup = null;
    [FormerlySerializedAs("_canvasFadeSpeed")]
    [SerializeField] private float _canvasFadeSpeed = 0.1f;

    [SerializeField] private GameObject _fewHeartsContainer = null;
    [FormerlySerializedAs("_moreHearthContainer")]
    [SerializeField] private GameObject _manyHeartsContainer = null;
    [FormerlySerializedAs("_moreHearthText")]
    [SerializeField] private TextMeshProUGUI _manyHeartsText = null;

    private List<GameObject> _spawnedHearth = new List<GameObject>();
    private float _fadeDelayTimer;

    public void SetHealth(int amount, int maxHp, int maxDisplayedHp)
    {
        if (amount < maxDisplayedHp)
        {
            _fewHeartsContainer.SetActive(true);
            _manyHeartsContainer.SetActive(false);

            PresentationHelpers.ResizeGameObjectList(_spawnedHearth, Mathf.Min(maxHp, maxDisplayedHp), _heartPrefab, _heartContainer);

            for (int i = 0; i < _spawnedHearth.Count; i++)
            {
                if (_spawnedHearth[i].TryGetComponent(out Image image))
                {
                    image.sprite = i < amount ? _filledSprite : _emptySprite;
                }
            }
        }
        else
        {
            _fewHeartsContainer.SetActive(false);
            _manyHeartsContainer.SetActive(true);

            _manyHeartsText.text = $"x{amount}";
        }
    }

    private void Update()
    {
        _fadeDelayTimer -= Time.deltaTime;

        if (_fadeDelayTimer <= 0)
        {
            _canvasGroup.alpha -= (Time.deltaTime * _canvasFadeSpeed);
        }
    }

    public void Show(float fadeDelay)
    {
        _canvasGroup.alpha = 1;
        _fadeDelayTimer = Mathf.Max(fadeDelay, _fadeDelayTimer);
    }
}
