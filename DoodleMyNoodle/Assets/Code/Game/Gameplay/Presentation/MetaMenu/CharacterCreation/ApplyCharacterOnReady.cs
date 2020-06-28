using System;
using TMPro;
using UnityEngine;
using UnityEngineX;

public class ApplyCharacterOnReady : GamePresentationBehaviour
{
    // todo : Apply Character Sprite
    public TMP_InputField NameField;
    public CharacterCreationStartingInventorySelection CharacterStartKit;

    protected override void OnGamePresentationUpdate() { }

    // Linked to Ready Button in editor
    public void ApplyCharacterSettings()
    {
        if (!gameObject.activeSelf)
            return;

        // todo name of the character

        // todo sprite of the character

        SimPlayerStartingInventorySelectionInput startInventoryInput = new SimPlayerStartingInventorySelectionInput(CharacterStartKit.CurrentlySelectedKit);
        SimWorld.SubmitInput(startInventoryInput);
    }
}