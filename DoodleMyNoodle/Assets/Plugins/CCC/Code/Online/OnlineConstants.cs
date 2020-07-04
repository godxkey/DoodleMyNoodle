using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OnlineConstants
{
    public const int PAQUET_SIZE = 1200; // in bytes
    public const int MAX_MESSAGE_SIZE = PAQUET_SIZE / 2; // in bytes
    public const int MAX_MESSAGE_SIZE_BITS = MAX_MESSAGE_SIZE * 8; // in bits
}