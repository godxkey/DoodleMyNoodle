using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugEditor
{
    public static void LogAssetIntegrity(string message)
    {
        Debug.Log($"✓ <i>Asset Integrity: {message}</i>");
    }
}
