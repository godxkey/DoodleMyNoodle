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
            canvas.ScheduleNextPaintJob(new UPaintCommonJobs.PaintLine()
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

        //[BurstCompile]
        //public struct LineJob : IJob
        //{
        //    public UPaintLayer Layer;
        //    [ReadOnly] public float2 StartCoordinates;
        //    [ReadOnly] public float2 EndCoordinates;
        //    [ReadOnly] public Color32 Color;
        //    [ReadOnly] public float Thickness;
        //    [ReadOnly] public float Gradient01;

        //    public void Execute()
        //    {
        //        float radius = Thickness / 2;
        //        float radiusSq = radius * radius;
        //        float2 lineMin = new float2(
        //            min(StartCoordinates.x, EndCoordinates.x),
        //            min(StartCoordinates.y, EndCoordinates.y));
        //        float2 lineMax = new float2(
        //            max(StartCoordinates.x, EndCoordinates.x),
        //            max(StartCoordinates.y, EndCoordinates.y));

        //        int2 drawMin = new int2((int)(floor(lineMin.x) - radius), (int)(floor(lineMin.y) - radius));
        //        int2 drawMax = new int2((int)(ceil(lineMin.x) + radius), (int)(ceil(lineMin.y) + radius));

        //        // clamp in canvas dimensions
        //        drawMin = max(drawMin, int2(0, 0));
        //        drawMax = min(drawMax, int2(Layer.Width, Layer.Height));


        //        for (int x = drawMin.x; x < drawMax.x; x++)
        //        {
        //            for (int y = drawMin.y; y < drawMax.y; y++)
        //            {
        //                float2 point = new float2(x, y);

        //                float2 vectorFromPointToLine = GetShortestVectorFromPointToLine(point, StartCoordinates, EndCoordinates);
        //                float2 intersectionPoint = point + vectorFromPointToLine;

        //                float distanceSq;
        //                if (inrange(intersectionPoint, lineMin, lineMax))
        //                {
        //                    distanceSq = lengthsq(vectorFromPointToLine);
        //                }
        //                else
        //                {
        //                    distanceSq = min(distancesq(point, StartCoordinates), distancesq(point, EndCoordinates));
        //                }

        //                if (distanceSq <= radiusSq)
        //                {
        //                    Color32 finalColor = Color;

        //                    // fade out the color near cicle edges
        //                    if (Gradient01 > 0.0001)
        //                    {
        //                        float blend01 = clamp(((distanceSq / radiusSq) - (1 - Gradient01)) / Gradient01,
        //                            0,
        //                            1);
        //                        finalColor.a = (byte)(finalColor.a * (1 - blend01));
        //                    }


        //                    Layer[x, y] = finalColor;
        //                }
        //            }
        //        }
        //    }

        //    static bool inrange(in float2 point, in float2 min, float2 max)
        //    {
        //        return point.x >= min.x && point.x <= max.x
        //            && point.y >= min.y && point.y <= max.y;
        //    }

        //    static float2 GetShortestVectorFromPointToLine(in float2 point, in float2 lineBegin, in float2 lineEnd)
        //    {
        //        float2 a = lineBegin;
        //        float2 b = point;
        //        float2 v = lineEnd - lineBegin;

        //        float n = v.x * (b.y - a.y) + v.y * (a.x - b.x);
        //        n /= (v.y * v.y) + (v.x * v.x);

        //        return  n * float2(v.y, -v.x);
        //    }
        //}
    }
}