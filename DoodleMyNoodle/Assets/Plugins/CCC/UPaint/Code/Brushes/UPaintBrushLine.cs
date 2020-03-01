using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace UPaintBrushes
{
    [System.Serializable]
    public class Line : IUPaintBursh
    {
        public float Gradient01;
        public int Thickness;

        private float2 _pressCoordinates;

        public void OnPress(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
            _pressCoordinates = context.CursorCoordinate;
        }

        public void OnHold(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
        }

        public void OnRelease(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
            canvas.ScheduleNextBrushJob(new UPaintCommonJobs.PaintLine()
            {
                StartCoordinates = _pressCoordinates,
                EndCoordinates = context.CursorCoordinate,
                Color = context.Color,
                Gradient01 = Gradient01,
                Thickness = Thickness,
                Layer = canvas.PreviewLayer
            });

            canvas.ScheduleBlendPreviewOntoMainLayer();
        }
    }
}