using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

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
            canvas.ScheduleNextPaintJob(new CircleJob()
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

        [BurstCompile]
        public struct CircleJob : IJob
        {
            public UPaintLayer Layer;
            [ReadOnly] public Color32 Color;
            [ReadOnly] public int2 CenterCoordinates;
            [ReadOnly] public int Width;
            [ReadOnly] public float Gradient01;

            public void Execute()
            {
                int xMin = CenterCoordinates.x - (Width / 2);
                int xMax = xMin + Width;
                int yMin = CenterCoordinates.y - (Width / 2);
                int yMax = yMin + Width;

                float edgeSqrMagnitude = (Width / 2) * (Width / 2);

                Vector2 pixelToCircleCenter;
                float sqrMag;
                Color32 finalColor;

                for (int x = xMin; x < xMax; x++)
                {
                    for (int y = yMin; y < yMax; y++)
                    {
                        pixelToCircleCenter.x = x - CenterCoordinates.x;
                        pixelToCircleCenter.y = y - CenterCoordinates.y;

                        sqrMag = pixelToCircleCenter.sqrMagnitude;

                        if (Layer.IsValidPixel(x, y) && sqrMag <= edgeSqrMagnitude)
                        {
                            finalColor = Color;

                            // fade out the color near cicle edges
                            if (Gradient01 > 0.0001)
                            {
                                float blend01 = math.clamp(((sqrMag / edgeSqrMagnitude) - (1 - Gradient01)) / Gradient01,
                                    0,
                                    1);
                                finalColor.a = (byte)(finalColor.a * (1 - blend01));
                            }

                            // if the pixel already had a color with some alpha, take the highest alpha of both
                            finalColor.a = (byte)math.max(finalColor.a, (uint)Layer[x, y].a);

                            // put color in layer
                            Layer[x, y] = finalColor;
                        }
                    }
                }
            }
        }
    }
}