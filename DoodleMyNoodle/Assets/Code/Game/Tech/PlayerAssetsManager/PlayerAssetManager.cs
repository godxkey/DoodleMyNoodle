using System;
using System.Collections.Generic;
using UnityEngineX;

[NetSerializable]
public struct NetMessagePlayerAssets
{
    [NetSerializable]
    public struct Data
    {
        public Guid Guid;
        public PlayerAssetType Type;
        public string Author;
        public DateTime UtcCreationTime;

        public byte[] AssetData;

        public Data(PlayerAsset playerAsset)
        {
            Guid = playerAsset.Guid;
            Type = playerAsset.Type;
            Author = playerAsset.Author;
            UtcCreationTime = playerAsset.UtcCreationTime;
            AssetData = playerAsset.Serialize();
        }
    }

    public Data[] Assets;
}

public abstract class PlayerAssetManager : GameSystem<PlayerAssetManager>
{
    public override bool SystemReady => true;

    public event Action<PlayerAsset> AssetCreated;

    protected Dictionary<Guid, PlayerAsset> _assetMap = new Dictionary<Guid, PlayerAsset>();

    protected override void OnDestroy()
    {
        base.OnDestroy();

        foreach (var item in _assetMap.Values)
        {
            item.Dispose();
        }

        _assetMap.Clear();
    }

    public void PublishAssetChanges(in Guid id)
    {
        if (!_assetMap.TryGetValue(id, out PlayerAsset playerAsset))
        {
            Log.Error($"There is no asset with the id {id}");
            return;
        }

        PublishAssetChangesInternal(playerAsset);
    }

    public T GetAsset<T>(in Guid id) where T : PlayerAsset
    {
        return GetAsset(id) as T;
    }

    public PlayerAsset GetAsset(in Guid id)
    {
        if (id == Guid.Empty)
            return null;

        if (_assetMap.TryGetValue(id, out PlayerAsset asset))
        {
            return asset;
        }

        return null;
    }

    /// <summary>
    /// Create a brand new asset
    /// </summary>
    public PlayerAsset CreateAsset(PlayerAssetType type)
    {
        return CreateAsset(AssetTypeFromEnum(type));
    }

    /// <summary>
    /// Create a brand new asset
    /// </summary>
    public T CreateAsset<T>() where T : PlayerAsset
    {
        return (T)CreateAsset(typeof(T));
    }

    /// <summary>
    /// Create a brand new asset
    /// </summary>
    private PlayerAsset CreateAsset(Type type)
    {
        if (AssetTypeToEnum(type) == PlayerAssetType.Invalid)
        {
            throw new Exception($"Unsupported asset type {type.GetPrettyName()}.");
        }

        PlayerAsset asset = CreateAssetInternal(Guid.NewGuid(), AssetTypeToEnum(type));

        asset.Author = Environment.UserName;
        asset.UtcCreationTime = DateTime.UtcNow;

        return asset;
    }

    /// <summary>
    /// Create an asset using an existing meta data set (Can be useful when an online player shares an asset he/she has created before)
    /// </summary>
    protected PlayerAsset CreateAssetInternal(Guid guid, PlayerAssetType assetType)
    {
        if (assetType == PlayerAssetType.Invalid)
        {
            throw new Exception($"Unsupported asset type.");
        }

        PlayerAsset asset = (PlayerAsset)Activator.CreateInstance(AssetTypeFromEnum(assetType), guid);

        _assetMap.Add(asset.Guid, asset);

        AssetCreated?.InvokeCatchException(asset);

        return asset;
    }

    protected abstract void PublishAssetChangesInternal(PlayerAsset playerAsset);

    protected void UpdateOrCreateLocalAssets(NetMessagePlayerAssets netMessage)
    {
        foreach (var item in netMessage.Assets)
        {
            if (item.Type == PlayerAssetType.Invalid)
            {
                logDiscardReason($"Invalid asset type");
                continue;
            }

            if (item.Guid == Guid.Empty)
            {
                logDiscardReason($"Invalid guid");
                continue;
            }

            if (item.AssetData == null)
            {
                logDiscardReason($"Null asset data");
                continue;
            }

            PlayerAsset asset = GetAsset(item.Guid);
            if (asset == null)
            {
                asset = CreateAssetInternal(item.Guid, item.Type);
            }

            asset.Author = item.Author;
            asset.UtcCreationTime = item.UtcCreationTime;
            asset.Load(item.AssetData);
        }

        void logDiscardReason(string reason)
        {
            Log.Warning($"{reason} in {nameof(NetMessagePlayerAssets)}. Discarding asset.");
        }
    }

    public static Type AssetTypeFromEnum(PlayerAssetType playerAssetType)
    {
        switch (playerAssetType)
        {
            default:
            case PlayerAssetType.Invalid:
                throw new ArgumentOutOfRangeException(nameof(playerAssetType));

            case PlayerAssetType.Doodle:
                return typeof(PlayerDoodleAsset);
        }
    }

    public static PlayerAssetType AssetTypeToEnum(Type playerAssetType)
    {
        if (playerAssetType == typeof(PlayerDoodleAsset))
        {
            return PlayerAssetType.Doodle;
        }

        return PlayerAssetType.Invalid;
    }

    /*
    private static SpriteRenderer s_testSpriteRenderer;
    private static PlayerDoodleAsset s_asset;

    private static Guid s_guid = new Guid(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);

    [ConsoleCommand]
    public static void Test_SetPlayerAssetVisible(bool visible = true)
    {
        if (s_testSpriteRenderer == null)
        {
            Log.Info("Create sprRenderer");
            SpriteRenderer sprRenderer = new GameObject("test").AddComponent<SpriteRenderer>();
            //await Task.Delay(1000);


            Log.Info("Get asset");
            s_asset = (PlayerDoodleAsset)Instance.GetAsset(s_guid);
            //await Task.Delay(1000);

            if (s_asset == null)
            {
                Log.Info("Asset not found, creating!");
                s_asset = (PlayerDoodleAsset)Instance.CreateAssetInternal(s_guid, PlayerAssetType.Doodle);
                s_asset.Author = Environment.UserName;
                s_asset.UtcCreationTime = DateTime.UtcNow;

                //await Task.Delay(1000);
            }

            Log.Info("Assigning sprite");
            sprRenderer.sprite = s_asset.Sprite;
            s_asset.SpriteUpdated += () =>
            {
                sprRenderer.sprite = s_asset.Sprite;
            };

            s_testSpriteRenderer = sprRenderer;

            sprRenderer.transform.localScale = Vector3.one * 2;
        }

        s_testSpriteRenderer.enabled = visible;
    }

    [ConsoleCommand]
    public static async void Test_PlayerAsset_LoadImage(int index = 0)
    {
        string path = null;
        switch (index)
        {
            case 0:
                path = @"D:\Projects\UPaint-Standalone\Test Exports\pogo.png";
                break;
            case 1:
                path = @"D:\Projects\UPaint-Standalone\Test Exports\pogo-happy.png";
                break;
            case 2:
                path = @"D:\Projects\UPaint-Standalone\Test Exports\test.png";
                break;
            case 3:
                path = @"D:\Projects\UPaint-Standalone\Test Exports\toto.png";
                break;
            default:
                break;
        }

        Test_SetPlayerAssetVisible(true);

        //await Task.Yield();
        //await Task.Yield();

        byte[] data = File.ReadAllBytes(path);

        s_asset.Load(data);

        Instance.PublishAssetChanges(s_asset.Guid);
    }*/
}