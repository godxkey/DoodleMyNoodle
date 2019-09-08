using System.Collections.Generic;

public struct GenericListEnumerable<EnumatorType>
{
    readonly IList<EnumatorType> _elements;
    public GenericListEnumerable(IList<EnumatorType> enties) { _elements = enties; }
    public Enumerator GetEnumerator() => new Enumerator(_elements);

    public struct Enumerator
    {
        readonly IList<EnumatorType> _entities;
        int _i;
        int _count;

        public Enumerator(IList<EnumatorType> entities)
        {
            _entities = entities;
            _count = _entities.Count;
            _i = -1;
        }

        public EnumatorType Current => _entities[_i];

        public bool MoveNext()
        {
            ++_i;
            return _i < _count;
        }
    }
}

public struct GenericListEnumerable<ListType, EnumatorType>
     where ListType : EnumatorType
{
    readonly IList<ListType> _entities;
    public GenericListEnumerable(List<ListType> enties) { _entities = enties; }
    public Enumerator GetEnumerator() => new Enumerator(_entities);

    public struct Enumerator
    {
        readonly IList<ListType> _entities;
        int _i;
        int _count;

        public Enumerator(IList<ListType> entities)
        {
            _entities = entities;
            _count = _entities.Count;
            _i = -1;
        }

        public EnumatorType Current => _entities[_i];

        public bool MoveNext()
        {
            ++_i;
            return _i < _count;
        }
    }
}