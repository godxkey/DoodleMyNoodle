using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class ReadyButton : GamePresentationBehaviour
{
    public Button Button;
    public TextMeshProUGUI Text;
    public Image Image;

    public string CantReadyYetText = "Wait";
    public string WaitingForReadyText = "Not Ready";
    public string ReadyText = "Ready";

    public Color WaitingForNextTurnColor = Color.white;
    public Color ReadyColor = Color.green;

    public UnityEvent OnReady = new UnityEvent();

    public enum TurnState
    {
        Ready,
        NotReady,
        NotMyTurn
    }

    private DirtyValue<TurnState> _state;

    public TurnState GetState() { return _state.Get(); }

    private float _updateTimer = 0;
    private const float UPDATE_DELAY = 0.5f;

    protected override void Awake()
    {
        Button.onClick.AddListener(OnReadyClicked);

        _updateTimer = UPDATE_DELAY;

        _state.Set(TurnState.NotMyTurn); // default value

        base.Awake();
    }

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorld.HasSingleton<NewTurnEventData>())
        {
            _updateTimer = UPDATE_DELAY; // reset timer as we're manually updating

            UpdateButtonState();
        }
        else
        {
            _updateTimer -= Time.deltaTime;
            if (_updateTimer <= 0)
            {
                _updateTimer = UPDATE_DELAY;

                UpdateButtonState();
            }
        }
    }

    private void UpdateButtonState()
    {
        if (SimWorldCache.CurrentTeam.Value == SimWorldCache.LocalPawnTeam.Value)
        {
            if (SimWorld.TryGetComponentData(SimWorldCache.LocalController, out ReadyForNextTurn ready) && ready.Value)
            {
                _state.Set(TurnState.Ready);
            }
            else
            {
                _state.Set(TurnState.NotReady);
            }
        }
        else
        {
            _state.Set(SimWorld.HasSingleton<GameReadyToStart>() ? TurnState.NotMyTurn : TurnState.NotReady );
        }

        if (_state.IsDirty)
        {
            switch (_state.Get())
            {
                case TurnState.Ready:
                    Text.text = ReadyText;
                    Image.color = ReadyColor;
                    break;
                case TurnState.NotReady:
                    Text.text = WaitingForReadyText;
                    Image.color = WaitingForNextTurnColor;
                    break;
                default:
                case TurnState.NotMyTurn:
                    Text.text = CantReadyYetText;
                    Image.color = WaitingForNextTurnColor;
                    break;
            }
            _state.Reset();
        }
    }

    private void OnReadyClicked()
    {
        if (_state.Get() == TurnState.NotMyTurn)
            return;

        OnReady?.Invoke();

        SimPlayerInputNextTurn simInput = new SimPlayerInputNextTurn(_state.Get() == TurnState.NotReady);
        SimWorld.SubmitInput(simInput);
    }
}
