using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;


public class UPaintTest : MonoBehaviour
{
    [Header("Links")]
    public UnityEngine.UI.RawImage RenderImage;

    [Header("Settings")]
    public Vector2Int RenderResolution = new Vector2Int(10, 10);
    public Texture2D RenderTexture;
    public Color DefaultPixeColor = new Color(1, 1, 1, 1);
    public FilterMode TextureFiltering;

    [Header("Data")]
    public UPaintLayer Layer;

    private void Start()
    {
        ResetRenderTexture();
        Layer.PaintJobs.AllPixels(Color.white);
    }

    void Update()
    {
        Layer.ApplyChangesIfPossible();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Layer.BeginGroupedPaint();
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            Layer.EndGroupedPaint();
        }

        if (Layer.IsProcessingJobs)
        {
            return;
        }


        if (Input.GetMouseButton(0))
        {
            Vector2Int pixelCoordinate = DisplayPositionToLayerCoordinate(Input.mousePosition);

            Layer.PaintJobs.Circle(pixelCoordinate, 50, 0.5f, Color.red);
        }

        if (Input.GetMouseButton(1))
        {
            Vector2Int pixelCoordinate = DisplayPositionToLayerCoordinate(Input.mousePosition);

            Layer.PaintJobs.Circle(pixelCoordinate, 50, 0, Color.white);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Layer.PaintJobs.AllPixels(Color.yellow);
        }
    }

    Vector2Int DisplayPositionToLayerCoordinate(Vector2 mousePosition)
    {
        // get render image display rect (from (0, 0) to (screenResX, screenResY))
        Rect renderImageRect = RenderImage.rectTransform.rect;
        renderImageRect.position = RenderImage.rectTransform.anchoredPosition;

        // get position in 'rect-space' (from (0,0) to (1,1))
        Vector2 rectSpacePosition = renderImageRect.GetPointInRectSpace(mousePosition);

        // scale position in 'pixel-space' (from (0,0) to (resX, resY))
        rectSpacePosition.Scale(new Vector2(Layer.Width, Layer.Height));

        // offset position half a pixel to account for pixel center
        rectSpacePosition -= new Vector2(0.5f, 0.5f);

        return rectSpacePosition.RoundedToInt();
    }

    [ContextMenu("reset render")]
    void ResetRenderTexture()
    {
        if (RenderTexture == null)
        {
            RenderTexture = new Texture2D(RenderResolution.x, RenderResolution.y, TextureFormat.RGBA32, mipChain: false);
            RenderTexture.name = "UPainTexture";
            RenderTexture.filterMode = TextureFiltering;
        }
        RenderTexture.Resize(RenderResolution.x, RenderResolution.y);

        RenderImage.texture = RenderTexture;

        if (Layer == null)
        {
            Layer = new UPaintLayer(RenderTexture);
        }
        else
        {
            Layer.UpdateToTextureChanges(RenderTexture);
        }
    }

    private void OnDestroy()
    {
        Layer.Dispose();
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