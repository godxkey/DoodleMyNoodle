using System;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public interface IUPaintBrushCanvasInterface
{
    UPaintLayer MainLayer { get; }
    UPaintLayer PreviewLayer { get; }
    void ScheduleNextPaintJob<T>(T job) where T : struct, IJob;
    void ScheduleBlendPreviewOntoMainLayer();
}

[System.Serializable]
public struct UPaintContext
{
    public int2 CursorCoordinate;
    public Color32 Color;
}

public class UPaintUnmanagedLayerContainer : IDisposable
{
    public Texture2D tempTexture;
    public UPaintUnmanagedLayerContainer(int width, int height)
    {
        tempTexture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false);

        Layer = new UPaintLayer()
        {
            Height = height,
            Width = width,
            Pixels = tempTexture.GetRawTextureData<Color32>(), /*new NativeArray<Color32>(height * width, Allocator.Persistent)*/
        };
    }
    public UPaintLayer Layer { get; private set; }

    public void Dispose()
    {
        //Layer.Pixels.Dispose();
    }
}

public class UPaintCanvas : IUPaintBrushCanvasInterface, IDisposable
{
    Texture2D _mainTexture;
    Texture2D _previewTexture;
    UPaintLayer _mainLayer;
    UPaintLayer _previewLayer;

    public List<UPaintUnmanagedLayerContainer> _layerHistory = new List<UPaintUnmanagedLayerContainer>();
    int _historyCount;
    const int MAX_HISTORY_LENGTH = 10;

    JobHandle _latestJob;
    bool _needApply;

    public UPaintCanvas(Texture2D mainTexture, Texture2D previewTexture)
    {
        _mainTexture = mainTexture;
        _previewTexture = previewTexture;

        void SetupLayer(ref UPaintLayer layer, Texture2D texture, in Color32 color)
        {
            layer = new UPaintLayer()
            {
                Pixels = texture.GetRawTextureData<Color32>(),
                Width = texture.width,
                Height = texture.height
            };
            ScheduleNextPaintJob(new UPaintJobs.PaintAllPixels()
            {
                Color = color,
                Layer = layer
            });
        }

        SetupLayer(ref _mainLayer, _mainTexture, Color.white);
        SetupLayer(ref _previewLayer, _previewTexture, new Color32(0, 0, 0, 0) /*transparent*/);
    }

    public void Dispose()
    {
        _latestJob.Complete();
        for (int i = 0; i < _layerHistory.Count; i++)
        {
            _layerHistory[i].Dispose();
        }
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
            // internally, unity uploads the texture to the GPU
            _mainTexture.Apply();
            _previewTexture.Apply();
            _needApply = false;
        }
    }

    public bool IsProcessingJobs => !_latestJob.IsCompleted;
    public int Width => _mainLayer.Width;
    public int Height => _mainLayer.Height;
    public bool IsValidPixel(int x, int y) => _mainLayer.IsValidPixel(x, y);
    public int AvailableUndos => _historyCount;
    UPaintLayer IUPaintBrushCanvasInterface.MainLayer => _mainLayer;
    UPaintLayer IUPaintBrushCanvasInterface.PreviewLayer => _previewLayer;

    public void PressBursh<T>(T brush, int2 pixelCoordinate, Color color) where T : IUPaintBursh
    {
        brush.OnPress(this, new UPaintContext() { CursorCoordinate = pixelCoordinate, Color = color });
    }
    public void HoldBursh<T>(T brush, int2 pixelCoordinate, Color color) where T : IUPaintBursh
    {
        brush.OnHold(this, new UPaintContext() { CursorCoordinate = pixelCoordinate, Color = color });
    }
    public void ReleaseBursh<T>(T brush, int2 pixelCoordinate, Color color) where T : IUPaintBursh
    {
        brush.OnRelease(this, new UPaintContext() { CursorCoordinate = pixelCoordinate, Color = color });
    }
    public void Undo()
    {
        SchedulePopHistoryIntoMainLayer();
    }

    void IUPaintBrushCanvasInterface.ScheduleNextPaintJob<T>(T job)
    {
        _latestJob = job.Schedule(_latestJob);
        _needApply = true;
    }

    void IUPaintBrushCanvasInterface.ScheduleBlendPreviewOntoMainLayer()
    {
        SchedulePushMainLayerToHistory();

        ScheduleNextPaintJob(new UPaintJobs.BlendLayerOneOntoLayerTwo()
        {
            LayerOne = _previewLayer,
            LayerTwo = _mainLayer
        });

        ScheduleNextPaintJob(new UPaintJobs.PaintAllPixels()
        {
            Color = new Color32(0, 0, 0, 0),
            Layer = _previewLayer
        });
        
    }

    void SchedulePushMainLayerToHistory()
    {
        if (_layerHistory.Count < MAX_HISTORY_LENGTH)
        {
            _layerHistory.Insert(0, new UPaintUnmanagedLayerContainer(_mainLayer.Width, _mainLayer.Height));
        }
        else
        {
            // move the last element back to the start
            _layerHistory.Insert(0, _layerHistory.Last());
            _layerHistory.RemoveLast();
        }

        if (_historyCount < MAX_HISTORY_LENGTH)
            _historyCount++;

        ScheduleNextPaintJob(new UPaintJobs.CopyLayerOneToTwo()
        {
            LayerOne = _mainLayer,
            LayerTwo = _layerHistory.First().Layer
        });
    }

    void SchedulePopHistoryIntoMainLayer()
    {
        if (_historyCount <= 0)
            return;

        ScheduleNextPaintJob(new UPaintJobs.CopyLayerOneToTwo()
        {
            LayerOne = _layerHistory.First().Layer,
            LayerTwo = _mainLayer
        });

        // put first layer last
        _layerHistory.MoveLast(0);
        _historyCount--;
    }

    void ScheduleNextPaintJob<T>(T job) where T : struct, IJob
    {
        _latestJob = job.Schedule(_latestJob);
        _needApply = true;
    }
}