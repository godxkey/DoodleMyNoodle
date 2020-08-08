using System;
using UnityEngine;
using UnityEngineX;

public class GameUI : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        [SerializeField] private HealthPoolDisplay _healthPool;
        [SerializeField] private InventoryDisplay _actionBar;
        [SerializeField] private TurnTeamDisplay _turnTeam;
        [SerializeField] private TimerBarDisplay _timerBar;
        [SerializeField] private ReadyButton _readyButton;

        public HealthPoolDisplay HealthPool => _healthPool;
        public InventoryDisplay ActionBar => _actionBar;
        public TurnTeamDisplay TurnTeam => _turnTeam;
        public TimerBarDisplay TimerBar => _timerBar;
        public ReadyButton ReadyButton => _readyButton;
    }

    public static Data Instance { get; private set; }

    [SerializeField] private Data _data;

    private void Awake()
    {
        Instance = _data;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}