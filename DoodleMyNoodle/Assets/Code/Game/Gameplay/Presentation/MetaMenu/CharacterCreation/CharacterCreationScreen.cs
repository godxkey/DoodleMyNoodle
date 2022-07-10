using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

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

        UIStateMachine.Instance.TransitionTo(UIStateType.Drawing);

        _readyButton.onClick.AddListener(ApplyCharacterSettings);

        _doodleAsset = PlayerAssetManager.Instance.CreateAsset<PlayerDoodleAsset>();
    }

    protected override void OnDestroy()
    {
        _readyButton.onClick.RemoveListener(ApplyCharacterSettings);

        base.OnDestroy();
    }

    public override void PresentationUpdate()
    {
        if (_doodelDraw.IsLastDoodleLoaded)
        {
            _screenContainer.SetActive(!SimWorld.HasSingleton<GameStartedTag>());

            // Temp fix - TODO change Ready Button use and do something better 
            // (using sim input and apply character settings directly does not work for some reason)
            ReadyButton readyButton = _readyButton.GetComponent<ReadyButton>();
            if (readyButton != null)
            {
                if (!_settingsApplied && (Cache.LocalPawn != Entity.Null) && (readyButton.GetState() == ReadyButton.TurnState.NotReady) && _doodelDraw.IsLibraryLoaded && s_consoleSkipCharacterCreation)
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

            UIStateMachine.Instance.TransitionTo(UIStateType.Gameplay);

            CursorOverlayService.Instance.ResetCursorToDefault();

            // Publish player asset (will sync across network)
            _doodleAsset.SetTexture(_doodelDraw.ExportCurrentDoodleTexture());
            PlayerAssetManager.Instance.PublishAssetChanges(_doodleAsset.Guid);

            // Set doodle
            SimPlayerInputSetPawnDoodle setPawnDoodleInput = new SimPlayerInputSetPawnDoodle(_doodleAsset.Guid, _characterIsLookingRightToggle.isOn);
            SimWorld.SubmitInput(setPawnDoodleInput);

            if (!s_consoleSkipCharacterCreation)
            {
                // record pawn name for future use
                //PromptDisplay.Instance.AskString("Enter your character name here :", (string characterName) =>
                //{
                //    // Set name
                //    SimPlayerInputSetPawnName startNameInput = new SimPlayerInputSetPawnName(characterName);
                //    SimWorld.SubmitInput(startNameInput);

                //    _settingsApplied = false;
                //});
            }
        }
    }

    #region Skip Character Creation
    private static bool s_consoleSkipCharacterCreation = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void StaticReset()
    {
        s_consoleSkipCharacterCreation = false;
    }

    [ConsoleCommand]
    public static bool SkipCharacterCreation() => s_consoleSkipCharacterCreation = true;
    #endregion
}