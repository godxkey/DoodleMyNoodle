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
    [FormerlySerializedAs("_emptyHearthPrefab")]
    [SerializeField] private GameObject _apPrefab = null;
    [FormerlySerializedAs("_hearthContainer")]
    [SerializeField] private Transform _apContainer = null;

    [FormerlySerializedAs("_emptyHearth")]
    [SerializeField] private Sprite _emptySprite = null;
    [FormerlySerializedAs("_filledHearth")]
    [SerializeField] private Sprite _filledSprite = null;

    [FormerlySerializedAs("_canvasGroup")]
    [SerializeField] private CanvasGroup _canvasGroup = null;
    [FormerlySerializedAs("_canvasFadeSpeed")]
    [SerializeField] private float _canvasFadeSpeed = 0.1f;

    [SerializeField] private GameObject _fewAPContainer = null;
    [FormerlySerializedAs("_moreHearthContainer")]
    [SerializeField] private GameObject _manyAPContainer = null;
    [FormerlySerializedAs("_moreHearthText")]
    [SerializeField] private TextMeshProUGUI _manyAPText = null;

    private List<GameObject> _spawnedAP = new List<GameObject>();
    private float _fadeDelayTimer;

    public void SetAP(int amount, int maxHp, int maxDisplayedHp)
    {
        if (amount < maxDisplayedHp)
        {
            _fewAPContainer.SetActive(true);
            _manyAPContainer.SetActive(false);

            PresentationHelpers.ResizeGameObjectList(_spawnedAP, Mathf.Min(maxHp, maxDisplayedHp), _apPrefab, _apContainer);

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
