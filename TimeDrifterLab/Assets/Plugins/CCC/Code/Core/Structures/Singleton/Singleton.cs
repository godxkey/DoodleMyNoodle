using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected static T instance;

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (ReferenceEquals(instance, this))
            instance = null;
    }
}
