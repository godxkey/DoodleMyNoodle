using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class ContextMenuActionDisplay : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;
    
    private Action _onClickCallback;

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        _onClickCallback.Invoke();
    }

    public void Init(string text, Action onClickCallback)
    {
        _onClickCallback = onClickCallback;
        _text.text = text;
    }
}