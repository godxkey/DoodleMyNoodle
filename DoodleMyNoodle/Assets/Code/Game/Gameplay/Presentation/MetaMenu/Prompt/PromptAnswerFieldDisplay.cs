using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class PromptAnswerFieldDisplay : MonoBehaviour
{
    [SerializeField] private Button _confirmButton;
    [SerializeField] private TextMeshProUGUI _text;

    public void Init(Action<string> onClickCallback)
    {
        _confirmButton.onClick.AddListener(() => { onClickCallback(_text.text); });
    }
}