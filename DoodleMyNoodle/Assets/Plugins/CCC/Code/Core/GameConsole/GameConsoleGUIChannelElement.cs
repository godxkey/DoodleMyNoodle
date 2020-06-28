using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class GameConsoleGUIChannelElement : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Text _text;
    [SerializeField] private Graphic _activeGraphic;
    [SerializeField] private Color _inactiveColor;
    [SerializeField] private Color _activeColor;

    public LogChannel Channel { get; private set; }

    private void Awake()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Log.SetChannelActive(Channel, !Channel.Active);
    }

    public void SetChannel(LogChannel logChannel)
    {
        Channel = logChannel;
        _text.text = logChannel.Name;
    }

    private void Update()
    {
        _activeGraphic.color = Channel.Active ? _activeColor : _inactiveColor;
    }
}