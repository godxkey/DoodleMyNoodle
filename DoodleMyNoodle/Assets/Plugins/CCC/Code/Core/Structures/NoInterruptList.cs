using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public interface IElementIndexHint
{
    int IndexHint { get; set; }
}

[Serializable]
public class NoInterruptList<T> where T : IElementIndexHint
{
    [SerializeField]
    private List<T> _items;
    private List<int> _iterators;

    public NoInterruptList()
    {
        _items = new List<T>();
        _iterators = new List<int>();
    }

    public NoInterruptList(IEnumerable<T> collection)
    {
        _items = new List<T>(collection);
        _iterators = new List<int>();

        for (int i = 0; i < _items.Count; i++)
        {
            var item = _items[i];
            item.IndexHint = i;
            _items[i] = item;
        }
    }

    public NoInterruptList(int capacity)
    {
        _items = new List<T>(capacity);
        _iterators = new List<int>();
    }

    public Enumerator GetEnumerator() => new Enumerator(_items, new IteratorManager(_iterators));

    public void Add(T element)
    {
        element.IndexHint = _items.Count;
        _items.Add(element);
    }

    public void AddRange(IEnumerable<T> range)
    {
        foreach (var item in range)
        {
            Add(item);
        }
    }

    public bool Remove(T element)
    {
        for (int i = Math.Min(element.IndexHint, _items.Count - 1); i >= 0; i--)
        {
            if (EqualityComparer<T>.Default.Equals(element, _items[i]))
            {
                RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    private void RemoveAt(int index)
    {
        _items.RemoveAt(index);

        // adjust all enumerators
        int c = _iterators.Count;
        for (int i = 0; i < c; i++)
        {
            if (_iterators[i] >= index)
            {
                --_iterators[i];
            }
        }
    }

    public struct Enumerator : IDisposable
    {
        private readonly List<T> _items;
        private int _iteratorId;
        private IteratorManager _iteratorManager;

        public Enumerator(List<T> items, IteratorManager iteratorManager)
        {
            _iteratorManager = iteratorManager;

            if (!_iteratorManager.TryTakeAvailableIterator(out _iteratorId))
            {
                _iteratorId = _iteratorManager.CreateNewIterator();
            }

            _items = items;
        }

        public Enumerator GetEnumerator() => this;

        public T Current => _items[_iteratorManager[_iteratorId]];

        public void Dispose()
        {
            _iteratorManager.ReleaseIterator(_iteratorId);
        }

        public bool MoveNext()
        {
            int itValue = _iteratorManager[_iteratorId];

            ++itValue;

            _iteratorManager[_iteratorId] = itValue;

            return itValue < _items.Count;
        }
    }

    public struct IteratorManager
    {
        private List<int> _iterators;

        private const int AVAILABLE = -2;

        public IteratorManager(List<int> iterators)
        {
            _iterators = iterators ?? throw new ArgumentNullException(nameof(iterators));
        }

        public int this[int index] { get => _iterators[index]; set => _iterators[index] = value; }

        public bool TryTakeAvailableIterator(out int index)
        {
            for (int i = 0; i < _iterators.Count; i++)
            {
                if (_iterators[i] == AVAILABLE)
                {
                    index = i;
                    _iterators[i] = -1;
                    return true;
                }
            }
            index = default;
            return false;
        }

        public int CreateNewIterator()
        {
            _iterators.Add(-1);
            return _iterators.Count - 1;
        }

        public void ReleaseIterator(int index)
        {
            _iterators[index] = AVAILABLE;
        }
    }
}