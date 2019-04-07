using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetMessage]
public class Testo
{
    public Popo somePopo;

    public string someString;

    [NetMessageAttributes.Ignore]
    public byte someByte;

    [NetMessageAttributes.Ignore]
    public bool someBool;
}

[NetMessage]
public struct Popo
{
    public float someFloat;
    public int someInt;

    [NetMessageAttributes.Ignore]
    uint someUInt;
}