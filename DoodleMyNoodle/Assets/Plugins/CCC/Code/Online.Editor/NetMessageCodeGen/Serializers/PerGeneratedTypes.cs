using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class NetMessageSerializersCodeGenerator
{
    static Type[] pregeneratedSerializers = new Type[]
    {
        typeof(Int32),
        typeof(Int16),
        typeof(UInt32),
        typeof(UInt16),
        typeof(String),
        typeof(Single),
        typeof(Char),
        typeof(Byte),
        typeof(Boolean),
        typeof(Vector2),
        typeof(Vector3),
        typeof(Vector4),
        typeof(Vector2Int),
        typeof(Vector3Int),
    };
}
