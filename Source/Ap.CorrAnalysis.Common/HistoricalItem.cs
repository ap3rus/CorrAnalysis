using System;
using System.Collections.Generic;

namespace Ap.CorrAnalysis.Common
{
    public class HistoricalItem<T> : IHistoricalItem<T>
    {
        public T this[int indexFromHead]
        {
            get
            {
                if (indexFromHead < Length)
                {
                    return _storage[_head - indexFromHead - 1];
                }

                return default(T);
            }
        }

        public T Latest
        {
            get { return _head > 0 ? _storage[_head - 1] : default(T); }
        }

        public int Length
        {
            get { return _head - _tail; }
        }

        private readonly int _maxHistoryLength;
        private readonly int _maxStorageSize;
        private T[] _storage;
        private int _head;
        private int _tail;

        public HistoricalItem(int maxHistoryLength, int maxStorageSize)
        {
            if (maxStorageSize < 0)
                throw new ArgumentOutOfRangeException("maxStorageSize", maxStorageSize,
                                                      "maxStorageSize must be greater than zero.");

            if (maxHistoryLength > maxStorageSize)
                throw new ArgumentOutOfRangeException("maxHistoryLength", maxHistoryLength,
                                                      "maxHistoryLength may not be larger then maxStorageSize");

            _maxHistoryLength = maxHistoryLength;
            _maxStorageSize = maxStorageSize;
            _storage = new T[maxStorageSize];
        }

        public void Add(T item)
        {
            item.AddReferenceIfAllocatable();
            _storage[_head++] = item;
            if (_head - _tail > _maxHistoryLength)
            {
                var oldItem = _storage[_tail];
                _storage[_tail] = default(T);
                _tail++;
                oldItem.ReleaseIfAllocatable();
            }

            if (_head >= _maxStorageSize) // when we reach the end, moving to the beginning
            {
                var newArray = new T[_maxStorageSize];
                Array.Copy(_storage, _tail, newArray, 0, _maxHistoryLength);
                _storage = newArray;
                _tail = 0;
                _head = _maxHistoryLength;
            }
        }

        public void For(Action<int, T> iterator)
        {
            for (var i = _tail; i < _head; i++)
            {
                iterator(i - _tail, _storage[i]);
            }
        }
    }
}