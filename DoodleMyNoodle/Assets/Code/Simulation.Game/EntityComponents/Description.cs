﻿using System;
using Unity.Collections;
using Unity.Entities;

[Serializable]
public struct Description : IComponentData
{
    // MAXIMUM 62 CHARACTERS !!!
    public FixedString128Bytes Value;
}
