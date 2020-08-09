using System;

public enum PlayerAssetType : byte
{
    Invalid = 0,
    Doodle,
}

public abstract class PlayerAsset : IDisposable
{
    public readonly Guid Guid;
    public readonly PlayerAssetType Type;

    /// <summary>
    /// The author's name
    /// </summary>
    public string Author;

    /// <summary>
    /// The date at which the asset was created
    /// </summary>
    public DateTime UtcCreationTime;

    protected PlayerAsset(Guid guid, PlayerAssetType type)
    {
        Guid = guid;
        Type = type;
    }

    public abstract string FileExtension { get; }

    public abstract void Dispose();
    public abstract void Load(byte[] data);
    public abstract byte[] Serialize();
}
