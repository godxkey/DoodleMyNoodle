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

    private string PawnNamePref
    {
        get
        {
            return PlayerPrefs.GetString("pawn-name", defaultValue: "");
        }
        set
        {
            PlayerPrefs.SetString("pawn-name", value);
            PlayerPrefs.Save();
        }
    }
    private string DoodlePathPref
    {
        get
        {
            return PlayerPrefs.GetString("doodle-path-2", defaultValue: "");
        }
        set
        {
            PlayerPrefs.SetString("doodle-path-2", value);
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

        // import previous doodle
        ImportDoodle(DoodlePathPref);

        _nameField.text = PawnNamePref;
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

        string directory = string.Empty;
        try
        {
            directory = Path.GetDirectoryName(DoodlePathPref);
        }
        catch { }

        string[] selectedFiles = StandaloneFileBrowser.OpenFilePanel("Select File", directory, allowedExtensions, false);
        if (selectedFiles.Length != 1)
        {
            return;
        }

        ImportDoodle(selectedFiles[0]);
    }

    private void ImportDoodle(string path)
    {
        if (File.Exists(path))
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);

                _doodleAsset.Load(bytes);
                _doodlePreview.enabled = true;
                DoodlePathPref = path;
            }
            catch (Exception e)
            {
                Log.Info($"Cannot import doodle at path {path}: {e.Message}");
            }
        }
    }

    private void ApplyCharacterSettings()
    {
        if ((GameUI.Instance.ReadyButton.GetState() == ReadyButton.TurnState.Ready) && gameObject.activeSelf)
        {
            if (_doodlePreview.enabled)
            {
                // Publish player asset (will sync across network)
                PlayerAssetManager.Instance.PublishAssetChanges(_doodleAsset.Guid);

                // Set doodle
                SimPlayerInputSetPawnDoodle setPawnDoodleInput = new SimPlayerInputSetPawnDoodle(_doodleAsset.Guid);
                SimWorld.SubmitInput(setPawnDoodleInput);
            }

            PawnNamePref = _nameField.text; // record pawn name for future use

            // Set name
            SimPlayerInputSetPawnName startNameInput = new SimPlayerInputSetPawnName(_nameField.text);
            SimWorld.SubmitInput(startNameInput);

            // Set starting kit
            SimPlayerInputSelectStartingInventory startInventoryInput = new SimPlayerInputSelectStartingInventory(_characterStartKit.CurrentlySelectedKit);
            SimWorld.SubmitInput(startInventoryInput);
        }
    }
}