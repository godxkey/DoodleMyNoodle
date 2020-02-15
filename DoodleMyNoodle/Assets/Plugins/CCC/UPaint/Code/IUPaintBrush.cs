using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public interface IUPaintBursh
{
    void OnPress(IUPaintBrushCanvasInterface canvas, in UPaintContext context);
    void OnHold(IUPaintBrushCanvasInterface canvas, in UPaintContext context);
    void OnRelease(IUPaintBrushCanvasInterface canvas, in UPaintContext context);
}