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
    public string WaitingForNextTurnText = "Next Turn";
    public string ReadyText = "Ready";

    public Color WaitingForNextTurnColor = Color.white;
    public Color ReadyColor = Color.green;

    private bool _isPressed = false;
    private bool _canBeClicked = false;
    private bool _wasMyTurn = false;

    private const float UPDATE_DELAY = 0.5f;

    protected override void Awake()
    {
        Button.onClick.AddListener(OnNextTurnClicked);

        this.DelayedCall(UPDATE_DELAY, () => UpdateButtonState());

        base.Awake();
    }

    public override void OnGameLateUpdate()
    {
        if (SimWorld.HasSingleton<NewTurnEventData>())
        {
            UpdateButtonState(false);
        }
    }

    private void UpdateButtonState(bool triggerDelay = true)
    {
        Entity pawnController = CommonReads.GetPawnController(SimWorld, SimWorldCache.LocalPawn);
        if (SimWorld.TryGetSingleton(out TurnCurrentTeam turnCurrentTeam))
        {
            if (SimWorld.TryGetComponentData(pawnController, out Team team))
            {
                // its our turn
                if (turnCurrentTeam.Value == team.Value)
                {
                    if (!_wasMyTurn)
                    {
                        _wasMyTurn = true;

                        _canBeClicked = true;
                    }
                }
                else
                {
                    // it's not my turn
                    if (_wasMyTurn)
                    {
                        _wasMyTurn = false;

                        _isPressed = false;
                        _canBeClicked = false;

                        Image.color = WaitingForNextTurnColor;
                        Text.text = WaitingForYourTurn;
                    }
                }
            }
        }

        if(triggerDelay)
            this.DelayedCall(UPDATE_DELAY, () => UpdateButtonState());
    }

    private void OnNextTurnClicked()
    {
        if (!_canBeClicked)
            return;

        _isPressed = !_isPressed;

        PlayerInputNextTurn simInput = new PlayerInputNextTurn(_isPressed);
        SimWorld.SubmitInput(simInput);

        if (_isPressed)
        {
            Text.text = ReadyText;
            Image.color = ReadyColor;
        }
        else
        {
            Text.text = WaitingForNextTurnText;
            Image.color = WaitingForNextTurnColor;
        }
    }
}
