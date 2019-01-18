using System;
using UnityEngine;

[Serializable]
public struct SerializableColor
{
    public float r;
    public float g;
    public float b;
    public float a;

    public SerializableColor(Color color) : this(color.r, color.g, color.b, color.a) { }
    public SerializableColor(float r, float g, float b) : this(r, g, b, 1) { }
    public SerializableColor(float r, float g, float b, float a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }

    public Color ToColor()
    {
        return new Color(r, g, b, a);
    }

    public static implicit operator Color(SerializableColor c)
    {
        return c.ToColor();
    }
}
