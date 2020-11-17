using UnityEngine;

public class SimAssetBankInstance : MonoBehaviour
{
    private static SimAssetBank.LookupData s_instance;

    [SerializeField] SimAssetBank _bank;

    private void Awake()
    {
        if(s_instance != null)
        {
            Debug.LogWarning($"{nameof(SimAssetBankInstance)} already exists.");
            return;
        }
        
        s_instance = new SimAssetBank.LookupData(_bank);
    }

    private void OnDestroy()
    {
        s_instance.Dispose();
        s_instance = null;
    }

    public static bool Ready => s_instance != null;

    public static SimAssetBank.Lookup GetLookup()
    {
        return new SimAssetBank.Lookup(s_instance);
    }
    
    public static SimAssetBank.JobLookup GetJobLookup()
    {
        return new SimAssetBank.JobLookup(s_instance);
    }
}
