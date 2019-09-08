using System;
using System.Collections.Generic;

public struct EntityEnumerable<T1>
{
    readonly List<SimEntity> _entities;
    public EntityEnumerable(List<SimEntity> enties) { _entities = enties; }
    public EntityEnumerator GetEnumerator() => new EntityEnumerator(_entities);

    public struct EntityEnumerator
    {
        readonly List<SimEntity> _entities;
        int _i;
        int _count;
        T1 _current;

        public EntityEnumerator(List<SimEntity> entities)
        {
            _entities = entities;
            _count = _entities.Count;
            _i = -1;
            _current = default;
        }

        public T1 Current => _current;

        public bool MoveNext()
        {
            do
            {
                ++_i;
                if (_i == _count)
                    return false;

                _current = _entities[_i].GetComponent<T1>();
            } while (_current == null);

            return true;
        }
    }
}

public struct EntityEnumerable<T1, T2>
{
    readonly List<SimEntity> _entities;
    public EntityEnumerable(List<SimEntity> enties) { _entities = enties; }
    public EntityEnumerator GetEnumerator() => new EntityEnumerator(_entities);

    public struct EntityEnumerator
    {
        readonly List<SimEntity> _entities;
        int _i;
        int _count;
        ValueTuple<T1, T2> _current;

        public EntityEnumerator(List<SimEntity> entities)
        {
            _entities = entities;
            _count = _entities.Count;
            _i = -1;
            _current = default;
        }

        public ValueTuple<T1, T2> Current => _current;

        public bool MoveNext()
        {
            do
            {
                ++_i;
                if (_i == _count)
                    return false;

                _current.Item1 = _entities[_i].GetComponent<T1>();
                _current.Item2 = _entities[_i].GetComponent<T2>();
            } while (_current.Item1 == null || _current.Item2 == null);

            return true;
        }
    }
}

public struct EntityEnumerable<T1, T2, T3>
{
    readonly List<SimEntity> _entities;
    public EntityEnumerable(List<SimEntity> enties) { _entities = enties; }
    public EntityEnumerator GetEnumerator() => new EntityEnumerator(_entities);

    public struct EntityEnumerator
    {
        readonly List<SimEntity> _entities;
        int _i;
        int _count;
        ValueTuple<T1, T2, T3> _current;

        public EntityEnumerator(List<SimEntity> entities)
        {
            _entities = entities;
            _count = _entities.Count;
            _i = -1;
            _current = default;
        }

        public ValueTuple<T1, T2, T3> Current => _current;

        public bool MoveNext()
        {
            do
            {
                ++_i;
                if (_i == _count)
                    return false;

                _current.Item1 = _entities[_i].GetComponent<T1>();
                _current.Item2 = _entities[_i].GetComponent<T2>();
                _current.Item3 = _entities[_i].GetComponent<T3>();
            } while (_current.Item1 == null || _current.Item2 == null || _current.Item3 == null);

            return true;
        }
    }
}

public struct EntityEnumerable<T1, T2, T3, T4>
{
    readonly List<SimEntity> _entities;
    public EntityEnumerable(List<SimEntity> enties) { _entities = enties; }
    public EntityEnumerator GetEnumerator() => new EntityEnumerator(_entities);

    public struct EntityEnumerator
    {
        readonly List<SimEntity> _entities;
        int _i;
        int _count;
        ValueTuple<T1, T2, T3, T4> _current;

        public EntityEnumerator(List<SimEntity> entities)
        {
            _entities = entities;
            _count = _entities.Count;
            _i = -1;
            _current = default;
        }

        public ValueTuple<T1, T2, T3, T4> Current => _current;

        public bool MoveNext()
        {
            do
            {
                ++_i;
                if (_i == _count)
                    return false;

                _current.Item1 = _entities[_i].GetComponent<T1>();
                _current.Item2 = _entities[_i].GetComponent<T2>();
                _current.Item3 = _entities[_i].GetComponent<T3>();
                _current.Item4 = _entities[_i].GetComponent<T4>();
            } while (_current.Item1 == null || _current.Item2 == null || _current.Item3 == null || _current.Item4 == null);

            return true;
        }
    }
}