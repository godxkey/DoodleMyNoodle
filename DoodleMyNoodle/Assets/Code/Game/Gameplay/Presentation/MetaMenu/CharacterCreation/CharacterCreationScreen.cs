using SFB;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class CharacterCreationScreen : GamePresentationSystem<CharacterCreationScreen>
{
    [SerializeField] private DoodleLibrary _library;
    [SerializeField] private GameObject _screenContainer;
    [SerializeField] private Button _readyButton;
    [SerializeField] private Toggle _characterIsLookingRightToggle;
    [SerializeField] private UPaintGUI _uPaint;

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
        _uPaint.Initialize(new Vector2Int(512, 512), FilterMode.Point, 10);
        _uPaint.AddLayer();
        Import();
    }

    protected override void OnDestroy()
    {
        _readyButton.onClick.RemoveListener(ApplyCharacterSettings);

        base.OnDestroy();
    }

    protected override void OnGamePresentationUpdate()
    {
        _screenContainer.SetActive(!SimWorld.HasSingleton<GameStartedTag>());
    }

    private void ApplyCharacterSettings()
    {
        if (!_settingsApplied)
        {
            _settingsApplied = true;

            _doodleAsset.SetTexture(_uPaint.GetLayerTexture(0));

            // Publish player asset (will sync across network)
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

            // We triggered everything, let's now save in the background while our things are done async
            Export();
        }
    }

    private void Import()
    {
        Texture2D lastDoodle = _library.GetLastDoodle();
        if (lastDoodle != null)
        {
            _doodleAsset.SetTexture(lastDoodle);
        }
    }

    private void Export()
    {
        _library.AddDoodle(_uPaint.GetLayerTexture(0));
    }
}