using UnityEngine;

public class SimAssetBankInstance : MonoBehaviour
{
    private static SimAssetBank s_instance;

    [SerializeField] SimAssetBank _bank;

    private void Awake()
    {
        if(s_instance != null)
        {
            Debug.LogWarning($"{nameof(SimAssetBankInstance)} already exists.");
            return;
        }
        
        s_instance = _bank;
    }

    private void OnDestroy()
    {
        s_instance = null;
    }


    public static SimAssetBank.LookUp GetLookup()
    {
        return s_instance.GetLookUp();
    }
}
