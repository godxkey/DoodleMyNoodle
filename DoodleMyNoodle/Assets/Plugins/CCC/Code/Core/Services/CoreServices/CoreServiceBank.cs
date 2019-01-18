using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CCC/Core Services/Core Service Bank")]
public class CoreServiceBank : ScriptableObject
{
    [Reorderable]
    [SerializeField] List<Object> coreServices;

    private const string ASSETNAME = "CCC/CoreServiceBank";

    public static CoreServiceBank LoadBank()
    {
        return (CoreServiceBank)Resources.Load(ASSETNAME);
    }

    public List<ICoreService> GetCoreServices()
    {
        List<ICoreService> list = new List<ICoreService>(coreServices.Count);
        foreach (Object serviceObj in coreServices)
        {
            if (serviceObj != null)
            {
                list.Add(GetCoreServiceFromObject(serviceObj));
            }
        }
        return list;
    }


    void OnValidate()
    {
        for (int i = 0; i < coreServices.Count; i++)
        {
            if (coreServices[i] == null) // Ok (the user is probably adding a new service into the array which temporarily adds a null entry)
            {
                continue;
            }

            if (GetCoreServiceFromObject(coreServices[i]) == null)
            {
                Debug.LogError("The object (" + coreServices[i].name + ") must either:\n1. Inherit from CoreService\n2. Be a GameObject prefab with a ICoreService component");
                coreServices[i] = null;
            }
        }
    }

    ICoreService GetCoreServiceFromObject(Object obj)
    {
        ICoreService service = obj as ICoreService;
        if (service == null)
        {
            GameObject serviceGameObject = obj as GameObject;
            if (serviceGameObject != null)
            {
                service = serviceGameObject.GetComponent<ICoreService>();
            }
        }
        return service;
    }
}
