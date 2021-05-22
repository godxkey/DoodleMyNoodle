using System;
using System.IO;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class DoodleDisplay : BindedPresentationEntityComponent
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private DirtyValue<Guid> _doodleAssetGuid;
    private DirtyRef<PlayerDoodleAsset> _doodleAsset;
    private Sprite _fallbackSprite = null;

    public SpriteRenderer SpriteRenderer => _spriteRenderer;

    protected override void Awake()
    {
        base.Awake();

        _fallbackSprite = _spriteRenderer.sprite;

        if (PlayerAssetManager.Instance != null)
        {
            PlayerAssetManager.Instance.AssetCreated += OnAssetCreated;
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_doodleAsset.Get() != null)
        {
            _doodleAsset.Get().SpriteUpdated -= SetSprite;
        }

        if (_doodleAsset.GetPrevious() != null)
        {
            _doodleAsset.GetPrevious().SpriteUpdated -= SetSprite;
        }

        if (PlayerAssetManager.Instance != null)
        {
            PlayerAssetManager.Instance.AssetCreated -= OnAssetCreated;
        }
    }

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorld.TryGetComponent(SimEntity, out DoodleId doodleId))
        {
            _doodleAssetGuid.Set(doodleId.Guid);
        }
        else
        {
            _doodleAssetGuid.Set(Guid.Empty);
        }

        if (_doodleAssetGuid.ClearDirty())
        {
            _doodleAsset.Set(PlayerAssetManager.Instance.GetAsset<PlayerDoodleAsset>(_doodleAssetGuid.Get()));
        }

        if (_doodleAsset.ClearDirty())
        {
            var previousDoodle = _doodleAsset.GetPrevious();
            var newDoodle = _doodleAsset.Get();

            if (previousDoodle != null)
            {
                previousDoodle.SpriteUpdated -= SetSprite;
            }

            if (newDoodle != null)
            {
                SetSprite(newDoodle.Sprite);
                newDoodle.SpriteUpdated += SetSprite;
            }
            else
            {
                SetSprite(null);
            }
        }
    }

    private void OnAssetCreated(PlayerAsset asset)
    {
        // If the newly created asset matches our guid, assign it!
        // This generally occurs when the simulation mentions a doodle BEFORE our PlayerAssetManager finishes downloading the asset its creator.

        if (_doodleAssetGuid.Get() == asset.Guid && asset is PlayerDoodleAsset doodleAsset)
        {
            _doodleAssetGuid.ClearDirty();
            _doodleAsset.Set(doodleAsset);
        }
    }

    private void SetSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite ?? _fallbackSprite;
    }

    [ConsoleCommand]
    private static void CheatSetPawnDoodle(string imagePath)
    {
        if (!GamePresentationCache.Instance.Ready)
        {
            return;
        }

        var simWorld = GamePresentationCache.Instance.SimWorld;
        Entity localPawn = GamePresentationCache.Instance.LocalPawn;

        // find current pawn's doodle asset
        Guid currentDoodleGuid = Guid.Empty;
        if (simWorld.TryGetComponent(localPawn, out DoodleId doodleId))
        {
            currentDoodleGuid = doodleId.Guid;
        }

        PlayerDoodleAsset playerDoodleAsset = PlayerAssetManager.Instance.GetAsset<PlayerDoodleAsset>(currentDoodleGuid);

        // If pawn's doodle doesn't exist, create a new one + submit a sim input to assign it
        if (playerDoodleAsset == null)
        {
            playerDoodleAsset = PlayerAssetManager.Instance.CreateAsset<PlayerDoodleAsset>();

            PresentationHelpers.SubmitInput(new SimPlayerInputSetPawnDoodle(playerDoodleAsset.Guid, true));
        }

        // Load image into doodle texture
        playerDoodleAsset.Load(File.ReadAllBytes(imagePath));

        // publish changes
        PlayerAssetManager.Instance.PublishAssetChanges(playerDoodleAsset.Guid);
    }
}