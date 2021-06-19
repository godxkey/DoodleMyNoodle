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
    [SerializeField] private GameObject _emptyHearthPrefab = null;
    [SerializeField] private Transform _hearthContainer = null;

    [SerializeField] private Sprite _emptyHearth = null;
    [SerializeField] private Sprite _filledHearth = null;

    [SerializeField] private CanvasGroup _canvasGroup = null;
    [SerializeField] private float _canvasFadeSpeed = 0.1f;

    [SerializeField] private GameObject _moreHearthContainer = null;
    [SerializeField] private TextMeshPro _moreHearthText = null;

    private List<GameObject> _spawnedHearth = new List<GameObject>();
    private float _fadeDelayTimer;
    private bool moreHearthDisplayActive = false;

    public void SetMaxHealth(int amount, int maxAmount)
    {
        if (_spawnedHearth.Count >= maxAmount)
        {
            UpdateMoreHealthDisplay(amount);
            return;
        }
        else
        {
            while (_spawnedHearth.Count != amount)
            {
                if (_spawnedHearth.Count == maxAmount)
                {
                    UpdateMoreHealthDisplay(amount);
                    return;
                }

                if (_spawnedHearth.Count < amount)
                {
                    _spawnedHearth.Add(Instantiate(_emptyHearthPrefab, _hearthContainer));
                    if (_spawnedHearth[_spawnedHearth.Count - 1].TryGetComponent(out Image image))
                    {
                        image.sprite = _filledHearth;
                    }
                }
                else if (_spawnedHearth.Count > amount)
                {
                    Destroy(_spawnedHearth[_spawnedHearth.Count - 1]);
                    _spawnedHearth.RemoveAt(_spawnedHearth.Count - 1);
                }
            }

            moreHearthDisplayActive = false;
            _moreHearthContainer.SetActive(false);
        }
    }

    public void SetHealth(int amount)
    {
        if (moreHearthDisplayActive)
        {
            if (amount < _spawnedHearth.Count)
            {
                moreHearthDisplayActive = false;
                _moreHearthContainer.SetActive(false);
            }
            else
            {
                UpdateMoreHealthDisplay(amount);
                return;
            }
        }

        for (int i = 0; i < amount; i++)
        {
            if (_spawnedHearth[i].TryGetComponent(out Image image))
            {
                image.sprite = _filledHearth;
            }
        }

        for (int i = amount; i < _spawnedHearth.Count; i++)
        {
            if (_spawnedHearth[i].TryGetComponent(out Image image))
            {
                image.sprite = _emptyHearth;
            }
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

    private void UpdateMoreHealthDisplay(int totalAmount)
    {
        int count = totalAmount - _spawnedHearth.Count;
        if (count > 0)
        {
            moreHearthDisplayActive = true;
            _moreHearthContainer.SetActive(true);
            _moreHearthText.text = "x" + (totalAmount - _spawnedHearth.Count);
        }
    }
}
