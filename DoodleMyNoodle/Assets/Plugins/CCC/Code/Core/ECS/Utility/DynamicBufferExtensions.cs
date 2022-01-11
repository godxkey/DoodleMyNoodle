using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

public static class DynamicBufferExtensions
{
    public static int IndexOf<T>(this DynamicBuffer<T> buffer, T element) where T : struct, IEquatable<T>
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i].Equals(element))
            {
                return i;
            }
        }
        return -1;
    }

    public static bool RemoveFirst<T>(this DynamicBuffer<T> buffer, T element) where T : struct, IEquatable<T>
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i].Equals(element))
            {
                buffer.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public static void RemoveAll<T>(this DynamicBuffer<T> buffer, T element) where T : struct, IEquatable<T>
    {
        for (int i = buffer.Length - 1; i >= 0; i--)
        {
            if (buffer[i].Equals(element))
            {
                buffer.RemoveAt(i);
            }
        }
    }

    public static bool IsValidIndex<T>(this DynamicBuffer<T> buffer, int index) where T : struct
        => index >= 0 && index < buffer.Length;
}
