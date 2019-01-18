using UnityEngine;

public partial struct CMath
{
    /// <summary>
    /// On the line y = ax + b, returns the closest point to 'distant point'
    /// <para>
    /// WARNING: a != 0 a != infinity
    /// </para>
    /// </summary>
    public static Vector2 GetClosestPointOnLine(float a, float b, Vector2 distantPoint)
    {
        //Nous cherchons la droite opposé (y = x/a + c) qui croise perpendiculerement la premiere droite
        float c = distantPoint.y - distantPoint.x / a;

        float x = (c - b) / (a - (1 / a));
        float y = a * x + b;
        return new Vector2(x, y);
    }

    public static Vector2 RandomVector2(float minLength, float maxLength, float minAngle, float maxAngle)
    {
        float angle = Random.Range(minAngle, maxAngle);
        float length = Random.Range(minLength, maxLength);

        return AngleToVector2D(angle) * length;
    }
    public static Vector2 RandomVector2(float minLength, float maxLength)
    {
        return RandomVector2(minLength, maxLength, 0, 360);
    }

    public static Vector2 ProjectVector(Vector2 vector, Vector2 normal)
    {
        float x = Vector2.Dot(vector, normal) / Vector2.Dot(normal, normal);
        return x * normal;
    }
    public static Vector3 ProjectVector(Vector3 vector, Vector3 normal)
    {
        return Vector3.Project(vector, normal);
    }
}
