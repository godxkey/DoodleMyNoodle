using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace UPaintBrushes
{
    [System.Serializable]
    public class FreeLine : IUPaintBursh
    {
        public float Gradient01;
        public int Thickness;

        private float2 _pressCoordinates;

        public void OnPress(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
            _pressCoordinates = context.CursorCoordinate;
            canvas.ScheduleNextBrushJob(new UPaintCommonJobs.PaintCircle()
            {
                CenterCoordinates = _pressCoordinates,
                Color = context.Color,
                Gradient01 = Gradient01,
                Width = Thickness,
                Layer = canvas.PreviewLayer
            });
        }

        public void OnHold(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
            float2 from = _pressCoordinates;
            float2 to = context.CursorCoordinate;

            // if the cursor moved
            if (any(to != from)) 
            {
                // paint line
                canvas.ScheduleNextBrushJob(new UPaintCommonJobs.PaintLine()
                {
                    StartCoordinates = from,
                    EndCoordinates = to,
                    Color = context.Color,
                    Gradient01 = Gradient01,
                    Thickness = Thickness,
                    Layer = canvas.PreviewLayer
                });
            }

            _pressCoordinates = context.CursorCoordinate;
        }

        public void OnRelease(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
            canvas.ScheduleBlendPreviewOntoMainLayer();
        }

    }
}