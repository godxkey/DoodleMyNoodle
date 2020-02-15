using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace UPaintCommonJobs
{
    [BurstCompile]
    public struct PaintCircle : IJob
    {
        public UPaintLayer Layer;
        [ReadOnly] public Color32 Color;
        [ReadOnly] public float2 CenterCoordinates;
        [ReadOnly] public float Width;
        [ReadOnly] public float Gradient01;

        public void Execute()
        {
            float radius = Width / 2;
            float radiusSq = radius * radius;

            UPaintOperations.ClampToLayerDimensions(
                min: CenterCoordinates - radius,
                max: CenterCoordinates + radius, Layer, 
                out int2 drawMin, 
                out int2 drawMax);

            for (int x = drawMin.x; x <= drawMax.x; x++)
            {
                for (int y = drawMin.y; y <= drawMax.y; y++)
                {
                    int2 point = new int2(x, y);

                    float distanceSq = distancesq(point, CenterCoordinates);

                    if (distanceSq <= radiusSq)
                    {
                        Color32 finalColor = Color;

                        // fade out the color near cicle edges
                        if (Gradient01 > 0.0001)
                        {
                            float blend01 = clamp(((distanceSq / radiusSq) - (1 - Gradient01)) / Gradient01,
                                0,
                                1);
                            finalColor.a = (byte)(finalColor.a * (1 - blend01));
                        }

                        // if the pixel already had a color with some alpha, take the highest alpha of both
                        finalColor.a = (byte)max(finalColor.a, (uint)Layer[x, y].a);

                        // put color in layer
                        Layer[x, y] = finalColor;
                    }
                }
            }
        }
    }

    [BurstCompile]
    public struct PaintLine : IJob
    {
        public UPaintLayer Layer;
        [ReadOnly] public float2 StartCoordinates;
        [ReadOnly] public float2 EndCoordinates;
        [ReadOnly] public Color32 Color;
        [ReadOnly] public float Thickness;
        [ReadOnly] public float Gradient01;

        public void Execute()
        {
            float radius = Thickness / 2;
            float radiusSq = radius * radius;
            float2 lineMin = new float2(
                min(StartCoordinates.x, EndCoordinates.x),
                min(StartCoordinates.y, EndCoordinates.y));
            float2 lineMax = new float2(
                max(StartCoordinates.x, EndCoordinates.x),
                max(StartCoordinates.y, EndCoordinates.y));

            UPaintOperations.ClampToLayerDimensions(
                min: lineMin - radius,
                max: lineMax + radius, Layer,
                out int2 drawMin,
                out int2 drawMax);

            for (int x = drawMin.x; x <= drawMax.x; x++)
            {
                for (int y = drawMin.y; y <= drawMax.y; y++)
                {
                    float2 point = new float2(x, y);

                    float2 vectorFromPointToLine = GetShortestVectorFromPointToLine(point, StartCoordinates, EndCoordinates);
                    float2 intersectionPoint = point + vectorFromPointToLine;

                    float distanceSq;
                    if (inrange(intersectionPoint, lineMin, lineMax))
                    {
                        distanceSq = lengthsq(vectorFromPointToLine);
                    }
                    else
                    {
                        distanceSq = min(distancesq(point, StartCoordinates), distancesq(point, EndCoordinates));
                    }

                    if (distanceSq <= radiusSq)
                    {
                        Color32 finalColor = Color;

                        // fade out the color near cicle edges
                        if (Gradient01 > 0.0001)
                        {
                            float blend01 = clamp(((distanceSq / radiusSq) - (1 - Gradient01)) / Gradient01,
                                0,
                                1);
                            finalColor.a = (byte)(finalColor.a * (1 - blend01));
                        }
                        
                        // if the pixel already had a color with some alpha, take the highest alpha of both
                        finalColor.a = (byte)max(finalColor.a, (uint)Layer[x, y].a);
                        
                        Layer[x, y] = finalColor;
                    }
                }
            }
        }

        static bool inrange(in float2 point, in float2 min, float2 max)
        {
            return point.x >= min.x && point.x <= max.x
                && point.y >= min.y && point.y <= max.y;
        }

        static float2 GetShortestVectorFromPointToLine(in float2 point, in float2 lineBegin, in float2 lineEnd)
        {
            float2 a = lineBegin;
            float2 b = point;
            float2 v = lineEnd - lineBegin;

            float n = v.x * (b.y - a.y) + v.y * (a.x - b.x);
            n /= (v.y * v.y) + (v.x * v.x);

            return n * float2(v.y, -v.x);
        }
    }

    [BurstCompile]
    public struct PaintPoint : IJob
    {
        public UPaintLayer Layer;
        public Color32 Color;
        public int2 Coordinates;

        public void Execute()
        {
            Layer[Coordinates] = Color;
        }
    }

    [BurstCompile]
    public struct PaintSquare : IJob
    {
        public UPaintLayer Layer;
        public Color32 Color;
        public int2 CenterCoordinates;
        public int Width;

        public void Execute()
        {
            int xStart = CenterCoordinates.x - (Width / 2);
            int xEnd = xStart + Width;
            int yStart = CenterCoordinates.y - (Width / 2);
            int yEnd = yStart + Width;

            for (int x = xStart; x < xEnd; x++)
            {
                for (int y = yStart; y < yEnd; y++)
                {
                    if (Layer.IsValidPixel(x, y))
                    {
                        Layer[x, y] = Color;
                    }
                }
            }
        }
    }

    [BurstCompile]
    public struct PaintAllPixels : IJob
    {
        public UPaintLayer Layer;
        public Color32 Color;

        public void Execute()
        {
            for (int i = 0; i < Layer.PixelCount; i++)
            {
                Layer[i] = Color;
            }
        }
    }
    
    [BurstCompile]
    public struct CopyLayerOneToTwo : IJob
    {
        [ReadOnly] public UPaintLayer LayerOne;
        [ReadOnly] public UPaintLayer LayerTwo;

        public void Execute()
        {
            LayerTwo.Height = LayerOne.Height;
            LayerTwo.Width = LayerOne.Width;
            for (int i = 0; i < LayerOne.PixelCount; i++)
            {
                LayerTwo[i] = LayerOne[i];
            }
        }
    }

    [BurstCompile]
    public struct BlendLayerOneOntoLayerTwo : IJob
    {
        [ReadOnly] public UPaintLayer LayerOne;
        public UPaintLayer LayerTwo;

        public void Execute()
        {
            for (int i = 0; i < LayerOne.PixelCount; i++)
            {
                LayerTwo[i] = UPaintOperations.AlphaBlendColorOneOntoTwo(LayerOne[i], LayerTwo[i]);
            }
        }
    }
}