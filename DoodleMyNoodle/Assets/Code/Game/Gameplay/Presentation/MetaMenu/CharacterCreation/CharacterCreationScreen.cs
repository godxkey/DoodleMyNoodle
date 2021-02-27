using SFB;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class CharacterCreationScreen : GamePresentationSystem<CharacterCreationScreen>
{
    [SerializeField] private GameObject _screenContainer;
    [SerializeField] private Button _readyButton;
    [SerializeField] private Toggle _characterIsLookingRightToggle;
    [SerializeField] private CharacterCreationDoodleDraw _doodelDraw;

    private PlayerDoodleAsset _doodleAsset;
    private bool _settingsApplied = false;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnGameStart()
    {
        base.OnGameStart();

        _readyButton.onClick.AddListener(ApplyCharacterSettings);

        _doodleAsset = PlayerAssetManager.Instance.CreateAsset<PlayerDoodleAsset>();
    }

    protected override void OnDestroy()
    {
        _readyButton.onClick.RemoveListener(ApplyCharacterSettings);

        base.OnDestroy();
    }

    protected override void OnGamePresentationUpdate()
    {
        if (_doodelDraw.IsLastDoodleLoaded)
        {
            _screenContainer.SetActive(!SimWorld.HasSingleton<GameStartedTag>());
        }
    }

    private void ApplyCharacterSettings()
    {
        if (!_settingsApplied)
        {
            _settingsApplied = true;

            // Publish player asset (will sync across network)
            _doodleAsset.SetTexture(_doodelDraw.ExportCurrentDoodleTexture());
            PlayerAssetManager.Instance.PublishAssetChanges(_doodleAsset.Guid);

            // Set doodle
            SimPlayerInputSetPawnDoodle setPawnDoodleInput = new SimPlayerInputSetPawnDoodle(_doodleAsset.Guid, _characterIsLookingRightToggle.isOn);
            SimWorld.SubmitInput(setPawnDoodleInput);

            // record pawn name for future use
            PromptDisplay.Instance.AskString("Enter your character name here :", (string characterName) =>
            {
                // Set name
                SimPlayerInputSetPawnName startNameInput = new SimPlayerInputSetPawnName(characterName);
                SimWorld.SubmitInput(startNameInput);

                _settingsApplied = false;
            });
        }
    }
}