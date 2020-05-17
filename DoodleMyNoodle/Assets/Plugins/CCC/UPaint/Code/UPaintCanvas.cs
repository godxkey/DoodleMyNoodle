using System;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

/// <summary>
/// Interface to the canvas that the brushes use
/// </summary>
public interface IUPaintBrushCanvasInterface
{
    UPaintLayer MainLayer { get; }
    UPaintLayer PreviewLayer { get; }
    void ScheduleNextBrushJob<T>(T job) where T : struct, IJob;
    void ScheduleBlendPreviewOntoMainLayer();
}


public class UPaintCanvas : IUPaintBrushCanvasInterface, IDisposable
{
    readonly Texture2D _mainTexture;
    readonly Texture2D _previewTexture;
    readonly UPaintLayer _mainLayer;
    readonly UPaintLayer _previewLayer;
    readonly List<UPaintUnmanagedLayerContainer> _layerHistory = new List<UPaintUnmanagedLayerContainer>();
    readonly int _maxHistoryLength;

    UPaintUnmanagedLayerContainer _tempLayer;
    JobHandle _latestJob;
    bool _needApply;

    public UPaintCanvas(Texture2D mainTexture, Texture2D previewTexture, int historyMaxLength, Color initColor)
    {
        _mainTexture = mainTexture;
        _previewTexture = previewTexture;
        _maxHistoryLength = historyMaxLength;

        void SetupLayer(ref UPaintLayer layer, Texture2D texture, in Color32 color)
        {
            layer = new UPaintLayer()
            {
                Pixels = texture.GetRawTextureData<Color32>(),
                Width = texture.width,
                Height = texture.height
            };
            ScheduleNextPaintJob(new UPaintCommonJobs.PaintAllPixels()
            {
                Color = color,
                Layer = layer
            });
        }

        SetupLayer(ref _mainLayer, _mainTexture, initColor);
        SetupLayer(ref _previewLayer, _previewTexture, new Color32(0, 0, 0, 0) /*transparent*/);
        
        _tempLayer = new UPaintUnmanagedLayerContainer(_mainLayer.Width, _mainLayer.Height);
    }

    public void Dispose()
    {
        _latestJob.Complete();
        for (int i = 0; i < _layerHistory.Count; i++)
        {
            _layerHistory[i].Dispose();
        }
        _tempLayer?.Dispose();
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

    public int AvailableUndos { get; private set; }
    public int AvailableRedos { get; private set; }
    public bool IsProcessingJobs => !_latestJob.IsCompleted;
    public int Width => _mainLayer.Width;
    public int Height => _mainLayer.Height;
    public bool IsValidPixel(int x, int y) => _mainLayer.IsValidPixel(x, y);

    public void PressBursh<T>(T brush, float2 pixelCoordinate, Color color) where T : IUPaintBursh
    {
        brush.OnPress(this, new UPaintContext() { CursorCoordinate = pixelCoordinate, Color = color });
    }
    public void HoldBursh<T>(T brush, float2 pixelCoordinate, Color color) where T : IUPaintBursh
    {
        brush.OnHold(this, new UPaintContext() { CursorCoordinate = pixelCoordinate, Color = color });
    }
    public void ReleaseBursh<T>(T brush, float2 pixelCoordinate, Color color) where T : IUPaintBursh
    {
        brush.OnRelease(this, new UPaintContext() { CursorCoordinate = pixelCoordinate, Color = color });
    }
    public void Undo()
    {
        ScheduleUndo();
    }
    public void Redo()
    {
        ScheduleRedo();
    }

    UPaintLayer IUPaintBrushCanvasInterface.MainLayer => _mainLayer;
    UPaintLayer IUPaintBrushCanvasInterface.PreviewLayer => _previewLayer;
    void IUPaintBrushCanvasInterface.ScheduleNextBrushJob<T>(T job)
    {
        _latestJob = job.Schedule(_latestJob);
        _needApply = true;
        AvailableRedos = 0;
    }
    void IUPaintBrushCanvasInterface.ScheduleBlendPreviewOntoMainLayer()
    {
        SchedulePushMainLayerToHistory();

        ScheduleNextPaintJob(new UPaintCommonJobs.BlendLayerOneOntoLayerTwo()
        {
            LayerOne = _previewLayer,
            LayerTwo = _mainLayer
        });

        ScheduleNextPaintJob(new UPaintCommonJobs.PaintAllPixels()
        {
            Color = new Color32(0, 0, 0, 0),
            Layer = _previewLayer
        });
        
    }

    void SchedulePushMainLayerToHistory()
    {
        if (_layerHistory.Count < _maxHistoryLength)
        {
            _layerHistory.Insert(0, new UPaintUnmanagedLayerContainer(_mainLayer.Width, _mainLayer.Height));
        }
        else
        {
            // move the last element back to the start
            _layerHistory.MoveFirst(_layerHistory.LastIndex());
        }

        if (AvailableUndos < _maxHistoryLength)
            AvailableUndos++;

        ScheduleNextPaintJob(new UPaintCommonJobs.CopyLayerOneToTwo()
        {
            LayerOne = _mainLayer,
            LayerTwo = _layerHistory.First().Layer
        });
    }

    void ScheduleRedo()
    {
        if (AvailableRedos <= 0)
            return;

        // copy main layer into temp (will be used for 'undo')
        ScheduleNextPaintJob(new UPaintCommonJobs.CopyLayerOneToTwo()
        {
            LayerOne = _mainLayer,
            LayerTwo = _tempLayer.Layer
        });

        // remove last history
        UPaintUnmanagedLayerContainer historyLayer = _layerHistory.Last();
        _layerHistory.RemoveLast();

        // copy last history into main layer
        ScheduleNextPaintJob(new UPaintCommonJobs.CopyLayerOneToTwo()
        {
            LayerOne = historyLayer.Layer,
            LayerTwo = _mainLayer
        });

        // set temp layer at start of history
        _layerHistory.Insert(0, _tempLayer);

        // move history layer we just used in temp
        _tempLayer = historyLayer;

        AvailableUndos++;
        AvailableRedos--;
    }

    void ScheduleUndo()
    {
        if (AvailableUndos <= 0)
            return;

        // copy main layer in temp (will be used for 'redo')
        ScheduleNextPaintJob(new UPaintCommonJobs.CopyLayerOneToTwo()
        {
            LayerOne = _mainLayer,
            LayerTwo = _tempLayer.Layer
        });

        // pop latest history
        UPaintUnmanagedLayerContainer historyLayer = _layerHistory.First();
        _layerHistory.RemoveFirst();

        // copy latest history into main layer
        ScheduleNextPaintJob(new UPaintCommonJobs.CopyLayerOneToTwo()
        {
            LayerOne = historyLayer.Layer,
            LayerTwo = _mainLayer
        });

        // move temp layer in back of history (will be used for 'redo')
        _layerHistory.Add(_tempLayer);

        // move history layer we just used in temp
        _tempLayer = historyLayer;

        AvailableUndos--;
        AvailableRedos++;
    }

    void ScheduleNextPaintJob<T>(T job) where T : struct, IJob
    {
        _latestJob = job.Schedule(_latestJob);
        _needApply = true;
    }
}