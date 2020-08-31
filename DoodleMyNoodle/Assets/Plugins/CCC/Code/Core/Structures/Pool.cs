using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : new()
{
    Queue<T> _queue;

    public Pool()
    {
        _queue = new Queue<T>();
    }

    public Pool(int capacity)
    {
        _queue = new Queue<T>(capacity);
    }

    public T Take()
    {
        if (_queue.Count > 0)
        {
            return _queue.Dequeue();
        }
        return new T();
    }

    public void Release(T obj)
    {
        _queue.Enqueue(obj);
    }
}


public abstract class PoolBehaviour<T> : MonoBehaviour where T : PoolBehaviour<T>.PoolItem
{
    protected Queue<T> deactivatedPool = new Queue<T>();

    private void AddToPool(T item)
    {
        item.OnDeactivateToPool();
        deactivatedPool.Enqueue(item);
    }

    protected T GetItem()
    {
        T item = null;
        if (deactivatedPool.Count <= 0)
        {
            item = CreateNewItem();
            item.pool = this;
        }
        else
        {
            item = deactivatedPool.Dequeue();
        }
        item.OnActivateFromPool();
        return item;
    }

    protected abstract T CreateNewItem();

    public abstract class PoolItem : MonoBehaviour
    {
        [System.NonSerialized]
        public PoolBehaviour<T> pool;

        protected void PutBackIntoPool()
        {
            pool.AddToPool((T)this);
        }
        public abstract void OnActivateFromPool();
        public abstract void OnDeactivateToPool();
    }
}
