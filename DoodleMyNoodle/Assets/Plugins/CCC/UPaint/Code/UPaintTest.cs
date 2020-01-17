using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;


public class UPaintTest : MonoBehaviour
{
    [Header("Links")]
    public UnityEngine.UI.RawImage MainRenderImage;
    public UnityEngine.UI.RawImage PreviewRenderImage;
    public UnityEngine.UI.RawImage[] HistoryRenderImages;

    [Header("Settings")]
    public Vector2Int RenderResolution = new Vector2Int(10, 10);
    public Color DefaultPixeColor = new Color(1, 1, 1, 1);
    public FilterMode TextureFiltering;

    [Header("Brushes")]
    public Color PaintColor;
    public UPaintBrushes.Circle CircleBrush;
    public UPaintBrushes.Line LineBrush;

    [Header("Data")]
    [ReadOnly] public Texture2D MainRenderTexture;
    [ReadOnly] public Texture2D PreviewRenderTexture;
    public UPaintCanvas Canvas;

    private void Start()
    {
        ResetRenderTexture();
    }

    void Update()
    {
        Canvas.ApplyChangesIfPossible();

        if (Input.GetMouseButtonDown(0))
        {
            int2 pixelCoordinate = DisplayPositionToLayerCoordinate(Input.mousePosition);

            Canvas.PressBursh(LineBrush, pixelCoordinate, PaintColor);
        }

        if (Input.GetMouseButtonUp(0))
        {
            int2 pixelCoordinate = DisplayPositionToLayerCoordinate(Input.mousePosition);

            Canvas.ReleaseBursh(LineBrush, pixelCoordinate, PaintColor);
        }

        if (Canvas.IsProcessingJobs)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Canvas.Undo();
        }

        if (Input.GetMouseButton(0))
        {
            int2 pixelCoordinate = DisplayPositionToLayerCoordinate(Input.mousePosition);

            Canvas.HoldBursh(LineBrush, pixelCoordinate, PaintColor);
        }

        for (int i = 0; i < HistoryRenderImages.Length; i++)
        {
            if(i < Canvas.AvailableUndos)
            {
                HistoryRenderImages[i].texture = Canvas._layerHistory[i].tempTexture;
                Canvas._layerHistory[i].tempTexture.Apply();
            }
            else
            {
                HistoryRenderImages[i].texture = null;
            }
        }
    }

    int2 DisplayPositionToLayerCoordinate(Vector2 mousePosition)
    {
        // get render image display rect (from (0, 0) to (screenResX, screenResY))
        Rect renderImageRect = MainRenderImage.rectTransform.rect;
        renderImageRect.position = MainRenderImage.rectTransform.anchoredPosition;

        // get position in 'rect-space' (from (0,0) to (1,1))
        Vector2 rectSpacePosition = renderImageRect.GetPointInRectSpace(mousePosition);

        // scale position in 'pixel-space' (from (0,0) to (resX, resY))
        rectSpacePosition.Scale(new Vector2(Canvas.Width, Canvas.Height));

        // offset position half a pixel to account for pixel center
        rectSpacePosition -= new Vector2(0.5f, 0.5f);

        return rectSpacePosition.RoundedToInt2();
    }

    [ContextMenu("reset render")]
    void ResetRenderTexture()
    {
        // Setup textures
        void SetupTexture(ref Texture2D texture, ref UnityEngine.UI.RawImage image, string name)
        {
            if (texture == null)
            {
                texture = new Texture2D(RenderResolution.x, RenderResolution.y, TextureFormat.RGBA32, mipChain: false);
                texture.name = name;
                texture.filterMode = TextureFiltering;
            }
            texture.Resize(RenderResolution.x, RenderResolution.y);

            image.texture = texture;
        }

        SetupTexture(ref MainRenderTexture, ref MainRenderImage, "UPaint Main Texture");
        SetupTexture(ref PreviewRenderTexture, ref PreviewRenderImage, "UPaint Preview Texture");

        // setup UPaint canvas
        Canvas?.Dispose();
        Canvas = new UPaintCanvas(MainRenderTexture, PreviewRenderTexture);
    }

    private void OnDestroy()
    {
        Canvas?.Dispose();
    }
}

public static class RectExtensions
{
    public static Vector2 GetPointInRectSpace(this in Rect rect, Vector2 position)
    {
        return new Vector2()
        {
            x = (position.x - rect.x) / rect.width,
            y = (position.y - rect.y) / rect.height
        };
    }
}