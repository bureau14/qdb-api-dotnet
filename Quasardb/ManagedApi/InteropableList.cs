using System;
using System.Collections;
using System.Collections.Generic;

namespace Quasardb.ManagedApi
{
    class InteropableList<T> : IEnumerable<T> where T : struct
    {
        T[] _buffer;
        int _count;

        public InteropableList(int initialCapacity)
        {
            _buffer = new T[initialCapacity];
            _count = 0;
        }

        public void Add(T value)
        {
            if (_count >= _buffer.Length)
                Array.Resize(ref _buffer, _buffer.Length * 2);

            _buffer[_count++] = value;
        }

        public void Clear()
        {
            _count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < _count; i++)
                yield return _buffer[i];
        }

        public T this[int index] => _buffer[index];

        public T[] Buffer => _buffer;

        public UIntPtr Count => (UIntPtr) _count;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}