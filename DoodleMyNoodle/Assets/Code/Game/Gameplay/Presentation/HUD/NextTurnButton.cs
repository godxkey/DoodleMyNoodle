using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;
using UnityEngine.UI;
using TMPro;

public class NextTurnButton : GamePresentationBehaviour
{
    public Button Button;
    public TextMeshProUGUI Text;
    public Image Image;

    public string WaitingForYourTurn = "Wait";
    public string WaitingForButtonPressText = "Next Turn";
    public string ReadyText = "Ready";

    public Color WaitingForNextTurnColor = Color.white;
    public Color ReadyColor = Color.green;

    private enum TurnState
    {
        Ready,
        NotReady,
        NotMyTurn
    }

    private DirtyValue<TurnState> _state;

    private float _updateTimer = 0;
    private const float UPDATE_DELAY = 0.5f;

    protected override void Awake()
    {
        Button.onClick.AddListener(OnNextTurnClicked);

        _updateTimer = UPDATE_DELAY;

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
            if(_updateTimer <= 0)
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
            _state.Set(TurnState.NotMyTurn);
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
                    Text.text = WaitingForButtonPressText;
                    Image.color = WaitingForNextTurnColor;
                    break;
                default:
                case TurnState.NotMyTurn:
                    Text.text = WaitingForYourTurn;
                    Image.color = WaitingForNextTurnColor;
                    break;
            }
            _state.Reset();
        }
    }

    private void OnNextTurnClicked()
    {
        if (_state.Get() == TurnState.NotMyTurn)
            return;

        SimPlayerInputNextTurn simInput = new SimPlayerInputNextTurn(_state.Get() == TurnState.NotReady);
        SimWorld.SubmitInput(simInput);
    }
}
