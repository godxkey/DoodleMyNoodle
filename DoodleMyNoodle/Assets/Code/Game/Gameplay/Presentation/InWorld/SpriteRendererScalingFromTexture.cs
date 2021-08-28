using System;
using UnityEngine;
using UnityEngineX;

public class SpriteRendererScalingFromTexture : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public float DefaultTexturePixelsPerUnit = 512;
    public float DefaultScaling = 1;

    public void SetScalingFromTexture(Sprite sprite) 
    {
        float newScaleRatio = (sprite.pixelsPerUnit * DefaultScaling) / DefaultTexturePixelsPerUnit;
        SpriteRenderer.transform.localScale = new Vector3(newScaleRatio, newScaleRatio, newScaleRatio);
        SpriteRenderer.sprite = sprite;
    }
}