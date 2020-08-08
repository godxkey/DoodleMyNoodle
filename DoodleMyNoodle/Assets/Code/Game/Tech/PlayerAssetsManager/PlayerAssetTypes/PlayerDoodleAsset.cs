using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngineX;

public class PlayerDoodleAsset : PlayerAsset
{
    public Texture2D Texture { get; private set; }
    public Sprite Sprite { get; private set; }
    public event Action<Sprite> SpriteUpdated;

    public override string FileExtension => ".png";

    public PlayerDoodleAsset(Guid guid)
        : base(guid, PlayerAssetType.Doodle)
    {
        Texture = new Texture2D(4, 4, TextureFormat.ARGB32, mipChain: false);
        Texture.wrapMode = TextureWrapMode.Clamp;
        RecreateSprite();
    }

    public override void Dispose()
    {
        UnityEngine.Object.Destroy(Sprite);
        UnityEngine.Object.Destroy(Texture);
        Texture = null;
    }

    public override void Load(byte[] data)
    {
        if (Texture != null)
        {
            Texture.LoadImage(data);
            RecreateSprite();
        }
    }

    private void RecreateSprite()
    {
        if (Sprite != null)
        {
            UnityEngine.Object.Destroy(Sprite);
        }

        int longerDimension = Mathf.Max(Texture.width, Texture.height);

        Sprite = Sprite.Create(Texture, new Rect(0, 0, Texture.width, Texture.height), Vector2.one * 0.5f, pixelsPerUnit: longerDimension);
        SpriteUpdated?.Invoke(Sprite);
    }

    public override byte[] Serialize()
    {
        return Texture.EncodeToPNG();
    }
}
