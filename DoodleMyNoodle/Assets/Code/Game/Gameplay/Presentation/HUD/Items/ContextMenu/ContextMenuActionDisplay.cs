using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class ContextMenuActionDisplay : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;

    public void Init(string text, Action onClickCallback)
    {
        _button.onClick.AddListener(() => { onClickCallback(); });
        _text.text = text;
    }
}