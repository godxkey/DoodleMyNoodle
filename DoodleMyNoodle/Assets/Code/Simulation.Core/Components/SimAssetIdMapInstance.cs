using UnityEngine;

public class SimAssetIdMapInstance : MonoBehaviour
{
    [SerializeField] SimAssetIdMap _simAssetIdMap;

    private void Awake()
    {
        if(s_instance != null)
        {
            Debug.LogWarning("SimAssetIdMapInstance already exists.");
            return;
        }
        s_instance = new SimAssetIdMap.LookUp(_simAssetIdMap);
    }

    private void OnDestroy()
    {
        s_instance = null;
    }

    private static SimAssetIdMap.LookUp s_instance;

    public static SimAssetIdMap.LookUp Get()
    {
        return s_instance;
    }
}
