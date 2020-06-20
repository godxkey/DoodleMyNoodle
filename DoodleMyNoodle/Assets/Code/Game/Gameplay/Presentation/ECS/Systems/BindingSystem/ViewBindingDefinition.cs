using System.Collections;
using UnityEngine;
using UnityX;

public class ViewBindingDefinition : MonoBehaviour
{
    public enum ESplitMode
    {
        FirstChildIsView,
        SecondChildIsView
    }
    public enum ETechType
    {
        GameObject,
        Entity
    }

    public ESplitMode SplitMode = ESplitMode.SecondChildIsView;
    public ETechType ViewTechType = ETechType.Entity;

    public SimAssetId GetSimAssetId()
    {
        GameObject simGO = GetSimGameObject();
        if (!simGO)
        {
            Log.Error($"[{nameof(ViewBindingDefinition)}] ({gameObject.name}) Failed to find Sim GameObject... The binding will not work.");
            return SimAssetId.Invalid;
        }

        SimAssetIdAuth assetIdAuth = simGO.GetComponent<SimAssetIdAuth>();

        if (!assetIdAuth)
        {
            Log.Error($"[{nameof(ViewBindingDefinition)}] ({gameObject.name}) The Sim GameObject {simGO.name} doesn't " +
                $"have any {nameof(SimAssetIdAuth)} component attached to it. The binding will not work.");
            return SimAssetId.Invalid;
        }

        return assetIdAuth.GetSimAssetId();
    }

    public GameObject GetGameObject(GameWorldType gameWorldType)
    {
        switch (gameWorldType)
        {
            case GameWorldType.Simulation:
                return GetSimGameObject();
            case GameWorldType.Presentation:
                return GetViewGameObject();
            default:
                return null;
        }
    }
    public GameObject GetSimGameObject()
    {
        return SplitMode == ESplitMode.FirstChildIsView ? GetChildAt(1) : GetChildAt(0);
    }
    public GameObject GetViewGameObject()
    {
        return SplitMode == ESplitMode.FirstChildIsView ? GetChildAt(0) : GetChildAt(1);
    }

    GameObject GetChildAt(int index)
    {
        if (transform.childCount > index)
            return transform.GetChild(index).gameObject;
        else
            return null;
    }
}
