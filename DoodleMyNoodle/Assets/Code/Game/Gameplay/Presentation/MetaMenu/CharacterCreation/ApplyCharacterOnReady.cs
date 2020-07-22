using System;
using TMPro;
using UnityEngine;
using UnityEngineX;

public class ApplyCharacterOnReady : GamePresentationBehaviour
{
    [SerializeField] private ReadyButton _readyButton;
    [SerializeField] private TMP_InputField _nameField;
    [SerializeField] private CharacterCreationStartingInventorySelection _characterStartKit;

    // todo : Apply Character Sprite

    protected override void Awake()
    {
        _readyButton.ButtonPressed += ApplyCharacterSettings;

        base.Awake();
    }

    private void ApplyCharacterSettings()
    {
        if ((_readyButton.GetState() == ReadyButton.TurnState.Ready) && (gameObject.activeSelf))
        {
            // todo sprite of the character

            SimPlayerCharacterNameInput startNameInput = new SimPlayerCharacterNameInput(_nameField.text);
            SimWorld.SubmitInput(startNameInput);

            SimPlayerStartingInventorySelectionInput startInventoryInput = new SimPlayerStartingInventorySelectionInput(_characterStartKit.CurrentlySelectedKit);
            SimWorld.SubmitInput(startInventoryInput);
        }
    }

    protected override void OnGamePresentationUpdate() { }
}