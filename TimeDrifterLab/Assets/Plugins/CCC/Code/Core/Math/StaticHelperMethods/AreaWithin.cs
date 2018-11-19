using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial struct CMath
{
    /// <summary>
    /// Trouve l'aire dans un ensemble de points
    /// </summary>
    /// <param name="points">Ensemble de points QUI DOIVENT ÊTRE ORDONNÉS</param>
    static public float Area2DWithin(Vector2[] points)
    {
        float totalAngle = 0;
        float crossZ = 0;

        for (int i = 0; i < points.Length; i++)
        {
            int i2 = (i + 1) % points.Length;
            int i3 = (i + 2) % points.Length;

            Vector2 v1 = points[i2] - points[i];
            Vector2 v2 = points[i3] - points[i2];
            crossZ += Vector3.Cross(v1, v2).z;
            totalAngle += Vector2.Angle(v1, v2);
        }

        return Area2DWithin(points, crossZ < 0);
    }
    /// <summary>
    /// Trouve l'aire dans un ensemble de points
    /// </summary>
    /// <param name="points">Ensemble de points QUI DOIVENT ÊTRE ORDONNÉS</param>
    /// <param name="sensHoraire">Partant du points 0 vers n, est-ce que la forme est tracé dans le sens horaire ?</param>
    static private float Area2DWithin(Vector2[] points, bool sensHoraire)
    {
        if (points.Length == 3)
        {
            bool areaAAjouter = (IsCounterClockwise(points[0], points[2], points[1]) == sensHoraire);
            float area = Area2DWithinTriangle(points[0], points[1], points[2]);
            if (areaAAjouter)
                return area;
            else
                return -area;
        }

        if (points.Length < 3)
            return 0;

        /////////////////////////////////////////
        //Shape intérieur
        /////////////////////////////////////////
        Vector2[] interiorShape = new Vector2[Mathf.CeilToInt((float)points.Length / 2f)];

        int u = 0;
        for (int i = 0; i < points.Length; i += 2)
        {
            interiorShape[u] = points[i];
            u++;
        }

        float interiorArea = Area2DWithin(interiorShape, sensHoraire);


        /////////////////////////////////////////
        //Les triangles extérieur
        /////////////////////////////////////////

        float totalArea = interiorArea;

        int lengthPair = points.Length;
        if (lengthPair % 2 != 0)
            lengthPair--;

        for (int y = 0; y < lengthPair; y += 2)
        {
            Vector2 pointA = points[y];
            Vector2 pointB = points[y + 1];
            Vector2 pointC = points[(y + 2) % points.Length];

            bool areaAAjouter = (IsCounterClockwise(pointA, pointC, pointB) == sensHoraire);

            float triangleArea = Area2DWithinTriangle(pointA, pointB, pointC);

            if (areaAAjouter)
                totalArea += triangleArea;
            else
                totalArea -= triangleArea;
        }


        /////////////////////////////////////////
        //Retour
        /////////////////////////////////////////

        return totalArea;
    }
    static private float Area2DWithinVectors(Vector2 vectorA, Vector2 vectorB, Vector2 vectorC)
    {
        //A² = (2ab + 2bc + 2ca – a² – b² – c²)/16
        float a = vectorA.sqrMagnitude;
        float b = vectorB.sqrMagnitude;
        float c = vectorC.sqrMagnitude;

        float areaSqr = (2 * ((a * b) + (b * c) + (c * a)) - (a * a) - (b * b) - (c * c)) / 16;

        if (areaSqr > 0.001f)
            return Mathf.Sqrt(areaSqr);
        return 0;
    }
    static public float Area2DWithinTriangle(Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
        return Area2DWithinVectors(pointC - pointA, pointB - pointA, pointC - pointB);
    }

    /// <summary>
    /// Retourne une valeur entre 0 et 1, représentant la ressemblance avec un cercle parfait.
    /// </summary>
    /// <param name="points">Ensemble de points QUI DOIVENT ÊTRE ORDONNÉS</param>
    static public float ResemblanceToCircle(float circleRadius, Vector2[] points)
    {
        float perfectArea = circleRadius * circleRadius * Mathf.PI;
        float area = Area2DWithin(points);

        return area / perfectArea;
    }


    static public bool IsCounterClockwise(Vector2 a, Vector2 b, Vector2 c)
    {
        return ((b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x)) > 0;
    }
    static public bool IsClockwise(Vector2 a, Vector2 b, Vector2 c)
    {
        return !IsCounterClockwise(a, b, c);
    }
}