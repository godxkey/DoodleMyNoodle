using System;
using System.Collections.Generic;

public struct EntityEnumerable<T1>
{
    ReadOnlyList<SimEntity> _entities;
    int _i;
    int _count;
    T1 _current;
    private SimEntity _simEntity;
    public EntityEnumerable(ReadOnlyList<SimEntity> entities)
    {
        _entities = entities;
        _count = _entities.Count;
        _i = -1;
        _current = default;
        _simEntity = null;
    }
    public EntityEnumerable<T1> GetEnumerator() => this;

    public T1 Current => _current;

    public bool MoveNext()
    {
        do
        {
            ++_i;
            if (_i == _count)
                return false;

            _simEntity = _entities[_i];
            if (_entities[_i])
            {
                _current = _simEntity.GetComponent<T1>();
            }
            else
            {
                _current = default;
            }
        } while (_current == null);

        return true;
    }
}

public struct EntityEnumerable<T1, T2>
{
    ReadOnlyList<SimEntity> _entities;
    int _i;
    int _count;
    ValueTuple<T1, T2> _current;
    private SimEntity _simEntity;
    public EntityEnumerable(ReadOnlyList<SimEntity> entities)
    {
        _entities = entities;
        _count = _entities.Count;
        _i = -1;
        _current = default;
        _simEntity = null;
    }
    public EntityEnumerable<T1, T2> GetEnumerator() => new EntityEnumerable<T1, T2>(_entities);

    public ValueTuple<T1, T2> Current => _current;

    public bool MoveNext()
    {
        do
        {
            ++_i;
            if (_i == _count)
                return false;

            _simEntity = _entities[_i];
            if (_simEntity)
            {
                _current.Item1 = _simEntity.GetComponent<T1>();
                _current.Item2 = _simEntity.GetComponent<T2>();
            }
            else
            {
                _current.Item1 = default;
                _current.Item2 = default;
            }
        } while (_current.Item1 == null || _current.Item2 == null);

        return true;
    }
}

public struct EntityEnumerable<T1, T2, T3>
{
    ReadOnlyList<SimEntity> _entities;
    int _i;
    int _count;
    ValueTuple<T1, T2, T3> _current;
    SimEntity _simEntity;
    public EntityEnumerable(ReadOnlyList<SimEntity> entities)
    {
        _entities = entities;
        _count = _entities.Count;
        _i = -1;
        _current = default;
        _simEntity = null;
    }
    public EntityEnumerable<T1, T2, T3> GetEnumerator() => new EntityEnumerable<T1, T2, T3>(_entities);

    public ValueTuple<T1, T2, T3> Current => _current;

    public bool MoveNext()
    {
        do
        {
            ++_i;
            if (_i == _count)
                return false;

            _simEntity = _entities[_i];
            if (_simEntity)
            {
                _current.Item1 = _simEntity.GetComponent<T1>();
                _current.Item2 = _simEntity.GetComponent<T2>();
                _current.Item3 = _simEntity.GetComponent<T3>();
            }
            else
            {
                _current.Item1 = default;
                _current.Item2 = default;
                _current.Item3 = default;
            }
        } while (_current.Item1 == null || _current.Item2 == null || _current.Item3 == null);

        return true;
    }
}

public struct EntityEnumerable<T1, T2, T3, T4>
{
    ReadOnlyList<SimEntity> _entities;
    int _i;
    int _count;
    ValueTuple<T1, T2, T3, T4> _current;
    SimEntity _simEntity;
    public EntityEnumerable(ReadOnlyList<SimEntity> entities)
    {
        _entities = entities;
        _count = _entities.Count;
        _i = -1;
        _current = default;
        _simEntity = null;
    }
    public EntityEnumerable<T1, T2, T3, T4> GetEnumerator() => new EntityEnumerable<T1, T2, T3, T4>(_entities);

    public ValueTuple<T1, T2, T3, T4> Current => _current;

    public bool MoveNext()
    {
        do
        {
            ++_i;
            if (_i == _count)
                return false;

            _simEntity = _entities[_i];
            if (_simEntity)
            {
                _current.Item1 = _simEntity.GetComponent<T1>();
                _current.Item2 = _simEntity.GetComponent<T2>();
                _current.Item3 = _simEntity.GetComponent<T3>();
                _current.Item4 = _simEntity.GetComponent<T4>();
            }
            else
            {
                _current.Item1 = default;
                _current.Item2 = default;
                _current.Item3 = default;
                _current.Item4 = default;
            }
        } while (_current.Item1 == null || _current.Item2 == null || _current.Item3 == null || _current.Item4 == null);

        return true;
    }
}
