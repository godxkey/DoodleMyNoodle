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
    [SerializeField] private UPaintGUI _uPaint;

    private PlayerDoodleAsset _doodleAsset;
    private bool _settingsApplied = false;

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


        _readyButton.onClick.AddListener(ApplyCharacterSettings);

        // import previous doodle
        _doodleAsset = PlayerAssetManager.Instance.CreateAsset<PlayerDoodleAsset>();
        ImportDoodle(DoodlePathPref);

        _uPaint.Initialize(new Vector2Int(512, 512), FilterMode.Point, 10);
        _uPaint.AddLayer();
        //_uPaint.ImportImage(_doodleAsset.Texture);
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

    private void ImportDoodle(string path)
    {
        // TODO : LIBRARY SYSTEM HERE

        if (File.Exists(path))
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(path);

                _doodleAsset.Load(bytes);
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
                PawnNamePref = characterName;

                // Set name
                SimPlayerInputSetPawnName startNameInput = new SimPlayerInputSetPawnName(characterName);
                SimWorld.SubmitInput(startNameInput);

                _settingsApplied = false;
            });

            // We triggered everything, let's now save in the background while our things are done async
            _uPaint.ExportToPNG();
        }
    }

    private void Export()
    {
        // TODO : Change for library system

        try
        {
            File.WriteAllBytes(GetExportPath(), _uPaint.ExportToPNG());
        }
        catch (Exception e)
        {
            Debug.LogError("Could not export and save last doodle made in character creation screen");
        }
    }

    private string GetExportPath()
    {
        // TODO : Change for library system

        string fullPath = Application.persistentDataPath;
        fullPath = fullPath.Replace('/', '\\');

        if (!fullPath.EndsWith("\\"))
            fullPath += "\\";

        fullPath += "lastDoodle";

        if (!fullPath.EndsWith(".png"))
            fullPath += ".png";

        return fullPath;
    }
}