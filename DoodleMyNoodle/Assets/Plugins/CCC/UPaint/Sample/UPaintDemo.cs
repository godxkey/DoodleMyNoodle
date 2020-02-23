using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;


public class UPaintDemo : MonoBehaviour
{
    public enum TestBrushes
    {
        Circle,
        Line,
        FreeLine,
        Fill
    }

    [Header("Links")]
    public UnityEngine.UI.RawImage MainRenderImage;
    public UnityEngine.UI.RawImage PreviewRenderImage;
    public UnityEngine.UI.Text UndoText;
    public UnityEngine.UI.Text RedoText;

    [Header("Settings")]
    public Vector2Int RenderResolution = new Vector2Int(10, 10);
    public Color DefaultPixeColor = new Color(1, 1, 1, 1);
    public FilterMode TextureFiltering;
    public int HistoryMaxLength = 10;

    [Header("Brushes")]
    public TestBrushes CurrentBrushType;
    public Color PaintColor;
    public UPaintBrushes.Circle CircleBrush;
    public UPaintBrushes.Line LineBrush;
    public UPaintBrushes.FreeLine FreeLineBrush;
    public UPaintBrushes.Fill FillBrush;

    [Header("Data")]
    public Texture2D MainRenderTexture;
    public Texture2D PreviewRenderTexture;
    public UPaintCanvas Canvas;

    private void Start()
    {
        ResetRenderTexture();
    }

    void Update()
    {
        UndoText.text = $"(u) Undos: {Canvas.AvailableUndos}";
        RedoText.text = $"(r) Redos: {Canvas.AvailableRedos}";

        Canvas.ApplyChangesIfPossible();

        if (Input.GetMouseButtonDown(0))
        {
            float2 pixelCoordinate = DisplayPositionToLayerCoordinate(Input.mousePosition);

            Canvas.PressBursh(CurrentBrush, pixelCoordinate, PaintColor);
        }

        if (Input.GetMouseButtonUp(0))
        {
            float2 pixelCoordinate = DisplayPositionToLayerCoordinate(Input.mousePosition);

            Canvas.ReleaseBursh(CurrentBrush, pixelCoordinate, PaintColor);
        }

        if (Input.GetMouseButton(0))
        {
            float2 pixelCoordinate = DisplayPositionToLayerCoordinate(Input.mousePosition);
            Canvas.HoldBursh(CurrentBrush, pixelCoordinate, PaintColor);
        }

        if (Canvas.IsProcessingJobs)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Canvas.Redo();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Canvas.Undo();
        }
    }

    IUPaintBursh CurrentBrush
    {
        get
        {
            switch (CurrentBrushType)
            {
                case TestBrushes.Circle:
                    return CircleBrush;
                case TestBrushes.Line:
                    return LineBrush;
                case TestBrushes.FreeLine:
                    return FreeLineBrush;
                case TestBrushes.Fill:
                    return FillBrush;
                default:
                    throw new System.Exception();
            }
        }
    }

    float2 DisplayPositionToLayerCoordinate(Vector2 mousePosition)
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

        return new float2(rectSpacePosition.x, rectSpacePosition.y);
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
        Canvas = new UPaintCanvas(MainRenderTexture, PreviewRenderTexture, HistoryMaxLength, DefaultPixeColor);
    }

    private void OnDestroy()
    {
        Canvas?.Dispose();
        FillBrush?.Dispose();
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