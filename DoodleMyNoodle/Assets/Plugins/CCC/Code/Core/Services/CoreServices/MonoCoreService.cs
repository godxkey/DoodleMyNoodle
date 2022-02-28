using System;
using UnityEngine;

public abstract class MonoCoreService<T> : MonoBehaviour, ICoreService where T : MonoCoreService<T>
{
    static public T Instance
    {
        get { return s_instance; }
        protected set
        {
            s_instance = value;
            if(s_instance != null)
            {
                DontDestroyOnLoad(s_instance.gameObject);
            }
        }
    }

    static private T s_instance;

    protected virtual void OnDestroy()
    {
        if (ReferenceEquals(this, Instance))
        {
            Instance = null;
        }
    }

    public abstract void Initialize(Action<ICoreService> onComplete);

    public virtual ICoreService ProvideOfficialInstance() // Can be overriden
    {
        if (Instance == null)
        {
            GameObject newServiceGameObject = Instantiate(gameObject);
            newServiceGameObject.name = "CoreService: " + name;
            DontDestroyOnLoad(newServiceGameObject);

            Instance = (T)newServiceGameObject.GetComponent(GetType());
        }

        return Instance;
    }
}
