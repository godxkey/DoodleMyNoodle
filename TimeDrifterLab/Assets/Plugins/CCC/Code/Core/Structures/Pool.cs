using System.Collections.Generic;
using UnityEngine;

public abstract class Pool<T> : MonoBehaviour where T : Pool<T>.PoolItem
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
        public Pool<T> pool;

        protected void PutBackIntoPool()
        {
            pool.AddToPool((T)this);
        }
        public abstract void OnActivateFromPool();
        public abstract void OnDeactivateToPool();
    }
}
