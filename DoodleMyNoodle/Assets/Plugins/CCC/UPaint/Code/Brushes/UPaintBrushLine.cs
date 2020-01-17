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


        private int2 _pressCoordinates;

        public void OnPress(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
            _pressCoordinates = context.CursorCoordinate;
        }

        public void OnHold(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
        }

        public void OnRelease(IUPaintBrushCanvasInterface canvas, in UPaintContext context)
        {
            canvas.ScheduleNextPaintJob(new LineJob()
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

        [BurstCompile]
        public struct LineJob : IJob
        {
            public UPaintLayer Layer;
            [ReadOnly] public int2 StartCoordinates;
            [ReadOnly] public int2 EndCoordinates;
            [ReadOnly] public Color32 Color;
            [ReadOnly] public int Thickness;
            [ReadOnly] public float Gradient01;

            public void Execute()
            {
                int radius = Thickness / 2;
                int2 min = new int2(
                    math.min(StartCoordinates.x, EndCoordinates.x),
                    math.min(StartCoordinates.y, EndCoordinates.y));
                int2 max = new int2(
                    math.max(StartCoordinates.x, EndCoordinates.x),
                    math.max(StartCoordinates.y, EndCoordinates.y));

                min -= radius;
                max += radius;

                min = math.max(min, int2(0, 0));
                max = math.min(max, int2(Layer.Width, Layer.Height));


                float distance;
                for (int x = min.x; x < max.x; x++)
                {
                    for (int y = min.y; y < max.y; y++)
                    {
                        distance = GetDistanceFromPointToLine(int2(x, y), StartCoordinates, EndCoordinates);
                        if(distance < radius)
                        {
                            Layer[x, y] = Color;
                        }
                    }
                }
            }

            float GetDistanceFromPointToLine(in int2 point, in int2 lineBegin, in int2 lineEnd)
            {
                int2 a = lineBegin;
                int2 b = point;
                int2 v = lineEnd - lineBegin;

                float n = v.x * (b.y - a.y) + v.y * (a.x - b.x);
                n /= (v.y * v.y) + (v.x * v.x);

                return length(n * float2(v));
            }
        }
    }
}