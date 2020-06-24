﻿using System;
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
}