using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;
using TMPro;

public class APBarDisplay : GameMonoBehaviour
{
    [SerializeField] private GameObject _apPrefab = null;
    [SerializeField] private Transform _apContainer = null;

    [SerializeField] private Sprite _emptySprite = null;
    [SerializeField] private Sprite _filledSprite = null;

    [SerializeField] private CanvasGroup _canvasGroup = null;
    [SerializeField] private float _canvasFadeSpeed = 0.1f;

    [SerializeField] private GameObject _fewAPContainer = null;
    [SerializeField] private GameObject _manyAPContainer = null;
    [SerializeField] private TextMeshProUGUI _manyAPText = null;

    private List<GameObject> _spawnedAP = new List<GameObject>();
    private float _fadeDelayTimer;

    public void SetAP(float amount, float maxAP, float maxDisplayedAP)
    {
        if (amount < maxDisplayedAP)
        {
            _fewAPContainer.SetActive(true);
            _manyAPContainer.SetActive(false);

            PresentationHelpers.ResizeGameObjectList(_spawnedAP, (int)Mathf.Min(maxAP, maxDisplayedAP), _apPrefab, _apContainer);

            for (int i = 0; i < _spawnedAP.Count; i++)
            {
                if (_spawnedAP[i].TryGetComponent(out Image image))
                {
                    image.sprite = i < amount ? _filledSprite : _emptySprite;
                }
            }
        }
        else
        {
            _fewAPContainer.SetActive(false);
            _manyAPContainer.SetActive(true);

            _manyAPText.text = $"{amount}";
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
