using SFB;
using System;
using System.IO;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
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

        _settingsApplied = false;
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

            // Temp fix - TODO change Ready Button use and do something better 
            // (using sim input and apply character settings directly does not work for some reason)
            ReadyButton readyButton = _readyButton.GetComponent<ReadyButton>();
            if (readyButton != null)
            {
                if (!_settingsApplied && (Cache.LocalPawn != Entity.Null) && (readyButton.GetState() == ReadyButton.TurnState.NotReady) && _doodelDraw.IsLibraryLoaded && SkipCharacterCreation)
                {
                    _readyButton.onClick.Invoke();
                }
            }
        }
    }

    private void ApplyCharacterSettings()
    {
        if (!_settingsApplied)
        {
            _settingsApplied = true;

            CursorOverlayService.Instance.ResetCursorToDefault();

            // Publish player asset (will sync across network)
            _doodleAsset.SetTexture(_doodelDraw.ExportCurrentDoodleTexture());
            PlayerAssetManager.Instance.PublishAssetChanges(_doodleAsset.Guid);

            // Set doodle
            SimPlayerInputSetPawnDoodle setPawnDoodleInput = new SimPlayerInputSetPawnDoodle(_doodleAsset.Guid, _characterIsLookingRightToggle.isOn);
            SimWorld.SubmitInput(setPawnDoodleInput);

            if (!SkipCharacterCreation)
            {
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

    #region Skip Character Creation
    private static bool s_consoleSkipCharacterCreation = false;

    [ConsoleVar(Save = ConsoleVarAttribute.SaveMode.PlayerPrefs, EnableGroup = SimulationCheats.LOCAL_PAWN_GROUP)]
    public static bool SkipCharacterCreation
    {
        get => s_consoleSkipCharacterCreation;
        set
        {
            s_consoleSkipCharacterCreation = value;
        }
    }
    #endregion
}