using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct UPaintLayerAccessor
{
    public NativeArray<Color32> ReadPixels;
    public NativeArray<Color32> WritePixels;
    public int Width;
    public int Height;

    public Color32 this[int index]
    {
        get => ReadPixels[index];
        set => WritePixels[index] = value;
    }
    public Color32 this[int x, int y]
    {
        get => ReadPixels[GetPixelIndexFromCoordinates(x, y)];
        set => WritePixels[GetPixelIndexFromCoordinates(x, y)] = value;
    }
    public Color32 this[in Vector2Int coordinates]
    {
        get => ReadPixels[GetPixelIndexFromCoordinates(coordinates)];
        set => WritePixels[GetPixelIndexFromCoordinates(coordinates)] = value;
    }
    public int PixelCount => ReadPixels.Length;

    public bool IsValidPixel(int x, int y) =>
        x >= 0 && x < Width &&
        y >= 0 && y < Height;

    public int GetPixelIndexFromCoordinates(in int x, in int y)
    {
        return x + (y * Height);
    }
    public int GetPixelIndexFromCoordinates(in Vector2Int coordinates)
    {
        return GetPixelIndexFromCoordinates(coordinates.x, coordinates.y);
    }
}

public class UPaintLayer : IDisposable
{
    Texture2D _texture;
    UPaintLayerAccessor _accessor;
    JobHandle _latestJob;
    bool _needApply;
    bool _applyPaintOntoTexture = true;

    public UPaintLayer(Texture2D texture)
    {
        UpdateToTextureChanges(texture);
    }

    public void UpdateToTextureChanges(Texture2D texture)
    {
        _texture = texture;
        NativeArray<Color32> textureData = _texture.GetRawTextureData<Color32>();
        _accessor = new UPaintLayerAccessor()
        {
            ReadPixels = textureData,
            WritePixels = new NativeArray<Color32>(textureData.Length, Allocator.Persistent),
            Width = _texture.width,
            Height = _texture.height
        };
        ResetGroupedPaint();
    }

    public void ForceCompleteJobsAndApply()
    {
        _latestJob.Complete();
        ApplyChangesIfPossible();
    }
    public void ApplyChangesIfPossible()
    {
        if (_needApply && _latestJob.IsCompleted)
        {
            _texture.Apply();
            _needApply = false;
        }
    }

    public void BeginGroupedPaint()
    {
        _applyPaintOntoTexture = false;
    }

    public void EndGroupedPaint()
    {
        _applyPaintOntoTexture = true; // TODO
        _accessor.WritePixels = _accessor.ReadPixels;

        ScheduleNextPaintJob(new UPaintJobs.ApplyLayerOneOntoLayerTwo()
        {
            LayerOne = _accessor.WritePixels,
            LayerTwo = _accessor.ReadPixels
        });

        ResetGroupedPaint();
    }

    void ResetGroupedPaint()
    {
        ScheduleNextPaintJob(new UPaintJobs.PaintAllPixels()
        {
            Color = new Color32(0, 0, 0, 0),
            Layer = _groupedPaint
        });
    }

    public bool IsProcessingJobs => !_latestJob.IsCompleted;
    public int Width => _accessor.Width;
    public int Height => _accessor.Height;
    public bool IsValidPixel(int x, int y) => _accessor.IsValidPixel(x, y);
    public JobBuilder PaintJobs => new JobBuilder(this);

    public struct JobBuilder
    {
        UPaintLayer _layer;
        public JobBuilder(UPaintLayer layer) { _layer = layer; }

        public JobHandle Point(Vector2Int coordinates, in Color32 color)
        {
            return _layer.ScheduleNextPaintJob(new UPaintJobs.PaintPoint()
            {
                Layer = _layer._accessor,
                Color = color,
                Coordinates = coordinates
            });
        }
        public JobHandle Square(Vector2Int coordinates, int thickness, in Color32 color)
        {
            return _layer.ScheduleNextPaintJob(new UPaintJobs.PaintSquare()
            {
                Layer = _layer._accessor,
                Color = color,
                CenterCoordinates = coordinates,
                Width = thickness
            });
        }
        public JobHandle Circle(Vector2Int coordinates, int thickness, float gradient01, in Color32 color)
        {
            return _layer.ScheduleNextPaintJob(new UPaintJobs.PaintCircle()
            {
                Layer = _layer._accessor,
                Color = color,
                CenterCoordinates = coordinates,
                Width = thickness,
                Gradient01 = gradient01
            });
        }
        public JobHandle AllPixels(in Color32 color)
        {
            return _layer.ScheduleNextPaintJob(new UPaintJobs.PaintAllPixels()
            {
                Layer = _layer._accessor.ReadPixels,
                Color = color
            });
        }
    }

    JobHandle ScheduleNextPaintJob<T>(T paintJob) where T : struct, IJob
    {
        _latestJob = paintJob.Schedule(_latestJob);
        _needApply = true;
        return _latestJob;
    }

    public void Dispose()
    {
        _accessor.WritePixels.Dispose();
    }
}