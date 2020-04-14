using System;
using System.Collections.Generic;
using UnityEngine;
using CCC.InspectorDisplay;

namespace CCC.Debug
{
    [Serializable]
    public class GraphDrawer
    {
        private const string SHADER_NAME = "CCC/Internal/GraphDrawer";
        private static Material s_material;

        public List<ColoredCurve> Curves = new List<ColoredCurve>();
        public List<ColoredPoint> Points = new List<ColoredPoint>();

        public Rect ValueDisplayRect;
        public Rect ScreenDisplayRect;

        public bool AutoZoomHorizontal = true;
        public bool AutoZoomVertical = true;

        [ShowIf(nameof(AutoSizeAny))]
        public float AutoZoomPadding = 15;

        private bool AutoSizeAny => AutoZoomVertical | AutoZoomHorizontal;

        public GraphDrawer()
        {
            ScreenDisplayRect = new Rect(0, 0, Screen.width, Screen.height);
            ValueDisplayRect = new Rect(position: Vector2.one * -5, size: Vector2.one * 10);
        }

        public void Draw()
        {
            if (GetMaterial() != null)
                GetMaterial().SetPass(0);

            if (AutoSizeAny)
            {
                Rect dataValueRect = CalculateValueRectFromData(AutoZoomPadding);
                if (AutoZoomHorizontal)
                {
                    ValueDisplayRect.xMin = dataValueRect.xMin;
                    ValueDisplayRect.xMax = dataValueRect.xMax;
                }
                if (AutoZoomVertical)
                {
                    ValueDisplayRect.yMin = dataValueRect.yMin;
                    ValueDisplayRect.yMax = dataValueRect.yMax;
                }
            }

            GL.PushMatrix();

            GL.Begin(GL.LINES);
            GL.Color(Color.white);
            GL.LoadPixelMatrix();

            // Draw curves
            for (int i = 0; i < Curves.Count; i++)
            {
                GL.Color(Curves[i].Color);
                for (int j = 1; j < Curves[i].Positions.Count; j++)
                {
                    AddLine_GS(Curves[i].Positions[j], Curves[i].Positions[j - 1]);
                }
            }

            // Draw points
            for (int i = 0; i < Points.Count; i++)
            {
                GL.Color(Points[i].color);
                AddCross_GS(Points[i].position);
            }
            GL.End();

            GL.PopMatrix();
        }

        Rect CalculateValueRectFromData()
        {
            Vector2 min = new Vector2(int.MaxValue, int.MaxValue);
            Vector2 max = new Vector2(int.MinValue, int.MinValue);

            void considerPoint(Vector2 p)
            {
                if (p.x < min.x)
                    min.x = p.x;
                if (p.y < min.y)
                    min.y = p.y;

                if (p.x > max.x)
                    max.x = p.x;
                if (p.y > max.y)
                    max.y = p.y;
            }

            // Check curves
            for (int i = 0; i < Curves.Count; i++)
                for (int j = 0; j < Curves[i].Positions.Count; j++)
                    considerPoint(Curves[i].Positions[j]);

            // Check points
            for (int i = 0; i < Points.Count; i++)
                considerPoint(Points[i].position);

            // Si ya aucun point, on met des valeurs par défaut
            if (max.x == int.MinValue)
            {
                min = Vector2.zero;
                max = Vector2.one;
            }

            return new Rect(position: min, size: max - min);
        }

        Rect CalculateValueRectFromData(float screenSpacePadding)
        {
            Rect valueRect = CalculateValueRectFromData();

            // Convert screen-space padding to value-space padding
            float verticalValuePadding = screenSpacePadding * (valueRect.height / ScreenDisplayRect.height);
            float horizontalValuePadding = screenSpacePadding * (valueRect.width / ScreenDisplayRect.width);

            valueRect.xMin -= horizontalValuePadding;
            valueRect.yMin -= verticalValuePadding;
            valueRect.xMax += horizontalValuePadding;
            valueRect.yMax += verticalValuePadding;

            return valueRect;
        }

        /// <summary>
        /// Graph Space
        /// </summary>
        void AddLine_GS(Vector2 a, Vector2 b)
        {
            AddLine_SS(GraphToScreenPos(a), GraphToScreenPos(b));
        }
        /// <summary>
        /// Screen Space
        /// </summary>
        void AddLine_SS(Vector2 a, Vector2 b)
        {
            GL.Vertex(a);
            GL.Vertex(b);
        }

        private const int CROSS_SIZE = 4;
        void AddCross_GS(Vector2 a)
        {
            AddCross_SS(GraphToScreenPos(a));
        }
        void AddCross_SS(Vector2 a)
        {
            AddLine_SS(a + Vector2.left * CROSS_SIZE, a + Vector2.right * CROSS_SIZE);
            AddLine_SS(a + Vector2.down * CROSS_SIZE, a + Vector2.up * CROSS_SIZE);
        }

        Vector2 GraphToScreenPos(Vector2 point)
        {
            return new Vector2(
                (point.x - ValueDisplayRect.xMin) / ValueDisplayRect.width * ScreenDisplayRect.width + ScreenDisplayRect.x,
                (point.y - ValueDisplayRect.yMin) / ValueDisplayRect.height * ScreenDisplayRect.height + (Screen.height - ScreenDisplayRect.yMax));
        }

        private static Material GetMaterial()
        {
            if (s_material == null)
            {
                Shader shader = Shader.Find(SHADER_NAME);
                if (shader == null)
                {
                    UnityEngine.Debug.LogError("No shader for the GraphDrawer");
                    return null;
                }
                s_material = new Material(shader);
            }
            return s_material;
        }
    }

}