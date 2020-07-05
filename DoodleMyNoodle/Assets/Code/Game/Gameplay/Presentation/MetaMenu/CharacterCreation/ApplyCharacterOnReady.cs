using System;
using TMPro;
using UnityEngine;
using UnityEngineX;

public class ApplyCharacterOnReady : GamePresentationBehaviour
{
    [SerializeField] private ReadyButton _readyButton;

    // todo : Apply Character Sprite
    public TMP_InputField NameField;
    public CharacterCreationStartingInventorySelection CharacterStartKit;

    protected override void Awake()
    {
        _readyButton.ButtonPressed += ApplyCharacterSettings;

        base.Awake();
    }

    private void ApplyCharacterSettings()
    {
        if ((_readyButton.GetState() == ReadyButton.TurnState.Ready) && (gameObject.activeSelf))
        {
            // todo name of the character

            // todo sprite of the character

            SimPlayerStartingInventorySelectionInput startInventoryInput = new SimPlayerStartingInventorySelectionInput(CharacterStartKit.CurrentlySelectedKit);
            SimWorld.SubmitInput(startInventoryInput);
        }
    }

    protected override void OnGamePresentationUpdate() { }
}