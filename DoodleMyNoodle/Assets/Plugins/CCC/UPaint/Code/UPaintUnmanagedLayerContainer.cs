using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


public class UPaintUnmanagedLayerContainer : IDisposable
{
    public UPaintUnmanagedLayerContainer(int width, int height)
    {
        Layer = new UPaintLayer()
        {
            Height = height,
            Width = width,
            Pixels = new NativeArray<Color32>(height * width, Allocator.Persistent)
        };
    }
    public UPaintLayer Layer { get; private set; }

    public void Dispose()
    {
        Layer.Pixels.Dispose();
    }
}