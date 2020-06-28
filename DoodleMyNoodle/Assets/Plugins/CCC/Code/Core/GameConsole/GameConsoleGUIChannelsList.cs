using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class GameConsoleGUIChannelsList : MonoBehaviour
{
    [SerializeField] private RectTransform _container;
    [SerializeField] private Toggle _toggle;
    [SerializeField] private GameConsoleGUIChannelElement _elementPrefab;

    private List<GameConsoleGUIChannelElement> _elements = new List<GameConsoleGUIChannelElement>();

    private void Awake()
    {
        _toggle.onValueChanged.AddListener((b) => gameObject.SetActive(b));
        gameObject.SetActive(_toggle.isOn);
        _container.GetComponentsInChildren(_elements);
    }

    private void OnEnable()
    {
        var channels = Log.GetChannels();
        
        int i = 0;
        for (; i < channels.Length; i++)
        {
            if(i >= _elements.Count)
            {
                var newElement = Instantiate(_elementPrefab, _container);
                _elements.Add(newElement);
            }

            _elements[i].gameObject.SetActive(true);
            _elements[i].SetChannel(channels[i]);
        }

        for (int r = _elements.Count - 1; r >= i; r--)
        {
            _elements[r].gameObject.SetActive(false);
        }
    }
}