using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class PromptAnswerDisplay : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;

    public void Init(string text, System.Action onClickCallback)
    {
        _button.onClick.AddListener(() => { onClickCallback(); });
        _text.text = text;
    }
}