using SFB;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class CharacterCreationScreen : GamePresentationBehaviour
{
    [SerializeField] private GameObject _screenContainer;
    [SerializeField] private Button _importDoodleButton;
    [SerializeField] private RawImage _doodlePreview;
    [SerializeField] private TMP_InputField _nameField;
    [SerializeField] private CharacterCreationStartingInventorySelection _characterStartKit;

    private PlayerDoodleAsset _doodleAsset;

    private string DoodleSearchFolder
    {
        get
        {
            return PlayerPrefs.GetString("doodle-path", defaultValue: "");
        }
        set
        {
            PlayerPrefs.SetString("doodle-path", value);
            PlayerPrefs.Save();
        }
    }

    protected override void Awake()
    {
        base.Awake();

    }

    public override void OnGameStart()
    {
        base.OnGameStart();

        _doodleAsset = PlayerAssetManager.Instance.CreateAsset<PlayerDoodleAsset>();
        _doodlePreview.enabled = false;
        _doodlePreview.texture = _doodleAsset.Texture;

        _importDoodleButton.onClick.AddListener(ImportDoodle);
        GameUI.Instance.ReadyButton.ButtonPressed += ApplyCharacterSettings;
    }

    protected override void OnDestroy()
    {
        if (GameUI.Instance?.ReadyButton)
            GameUI.Instance.ReadyButton.ButtonPressed -= ApplyCharacterSettings;

        base.OnDestroy();
    }

    protected override void OnGamePresentationUpdate()
    {
        _screenContainer.SetActive(!SimWorld.HasSingleton<GameStartedTag>());
    }

    private void ImportDoodle()
    {
        ExtensionFilter[] allowedExtensions = new ExtensionFilter[]
        {
            new ExtensionFilter("Images", "png", "jpg")
        };

        string[] selectedFiles = StandaloneFileBrowser.OpenFilePanel("Select File", DoodleSearchFolder, allowedExtensions, false);
        if (selectedFiles.Length != 1)
        {
            return;
        }

        DoodleSearchFolder = Path.GetDirectoryName(selectedFiles[0]); // set search folder for next search

        byte[] bytes = File.ReadAllBytes(selectedFiles[0]);

        _doodleAsset.Load(bytes);
        _doodlePreview.enabled = true;
    }

    private void ApplyCharacterSettings()
    {
        if ((GameUI.Instance.ReadyButton.GetState() == ReadyButton.TurnState.Ready) && (gameObject.activeSelf))
        {
            // Publish player asset (will sync across network)
            PlayerAssetManager.Instance.PublishAssetChanges(_doodleAsset.Guid);

            // Set doodle
            SimPlayerInputSetPawnDoodle setPawnDoodleInput = new SimPlayerInputSetPawnDoodle(_doodleAsset.Guid);
            SimWorld.SubmitInput(setPawnDoodleInput);

            // Set name
            SimPlayerInputSetPawnName startNameInput = new SimPlayerInputSetPawnName(_nameField.text);
            SimWorld.SubmitInput(startNameInput);

            // Set starting kit
            SimPlayerInputSelectStartingInventory startInventoryInput = new SimPlayerInputSelectStartingInventory(_characterStartKit.CurrentlySelectedKit);
            SimWorld.SubmitInput(startInventoryInput);
        }
    }
}