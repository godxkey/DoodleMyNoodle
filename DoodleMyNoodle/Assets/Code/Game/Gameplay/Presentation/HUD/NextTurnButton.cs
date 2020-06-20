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

    private bool _hasPressedOnce = false;

    protected override void Awake()
    {
        Button.onClick.AddListener(OnNextTurnClicked);

        base.Awake();
    }

    public override void OnGameLateUpdate()
    {
        if (SimWorld.HasSingleton<NewTurnEventData>())
        {
            _hasPressedOnce = false;

            Image.color = WaitingForNextTurnColor;

            if (SimWorld.TryGetSingleton(out TurnCurrentTeam turnCurrentTeam))
            {
                switch ((TeamAuth.DesignerFriendlyTeam)turnCurrentTeam.Value)
                {
                    case TeamAuth.DesignerFriendlyTeam.Player:
                        Text.text = WaitingForNextTurnText;
                        break;
                    case TeamAuth.DesignerFriendlyTeam.Baddies:
                        Text.text = WaitingForYourTurn;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void OnNextTurnClicked()
    {
        if (!_hasPressedOnce)
        {
            SimPlayerInputNextTurn simInput = new SimPlayerInputNextTurn();
            SimWorld.SubmitInput(simInput);

            Text.text = ReadyText;
            Image.color = ReadyColor;

            _hasPressedOnce = true;
        }
    }
}
