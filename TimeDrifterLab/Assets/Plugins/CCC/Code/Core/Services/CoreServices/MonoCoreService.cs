using System;
using UnityEngine;

public abstract class MonoCoreService<T> : MonoBehaviour, ICoreService where T : MonoCoreService<T>
{
    static public T Instance { get; private set; }

    protected virtual void OnDestroy()
    {
        if(ReferenceEquals(this, Instance))
        {
            Instance = null;
        }
    }

    public virtual void Initialize(Action onComplete) { onComplete(); }

    public ICoreService ProvideOfficialInstance() // Can be overriden
    {
        GameObject newServiceGameObject = Instantiate(gameObject);
        newServiceGameObject.name = "CoreService: " + name;
        DontDestroyOnLoad(newServiceGameObject);

        Instance = (T)newServiceGameObject.GetComponent(GetType());

        return Instance;
    }
}
