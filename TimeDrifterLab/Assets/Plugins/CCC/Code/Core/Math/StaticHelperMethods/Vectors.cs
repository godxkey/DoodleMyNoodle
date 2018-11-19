using UnityEngine;

public partial struct CMath
{
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
