using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace UPaintBrushes
{
    [System.Serializable]
    public class Circle : IUPaintBursh
    {
        public float Gradient01;
        public int Width;

        public void OnPress(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
            // nothing
        }

        public void OnHold(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
            canvas.ScheduleNextBrushJob(new UPaintCommonJobs.PaintCircle()
            {
                CenterCoordinates = context.CursorCoordinate,
                Color = context.Color,
                Gradient01 = Gradient01,
                Width = Width,
                Layer = canvas.PreviewLayer
            });
        }

        public void OnRelease(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
            canvas.ScheduleBlendPreviewOntoMainLayer();
        }

    }
}