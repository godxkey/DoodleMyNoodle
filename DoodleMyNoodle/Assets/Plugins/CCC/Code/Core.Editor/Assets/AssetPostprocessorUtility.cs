using System;
using UnityEngine;
using UnityEngineX;

public static class AssetPostprocessorUtility
{
    public static bool ExitImportLoop(string[] importedAssets, string assetPath, ref int counter)
    {
        if (importedAssets.Contains(assetPath))
        {
            counter++;
            if (counter >= 250)
            {
                Log.Warning("Exiting import loop on asset: " + assetPath);
                return true;
            }
        }
        else
        {
            counter = 0;
        }

        return false;
    }
}