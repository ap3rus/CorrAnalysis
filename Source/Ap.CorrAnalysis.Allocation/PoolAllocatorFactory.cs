using System;

namespace Ap.CorrAnalysis.Allocation
{
    public class PoolAllocatorFactory : IAllocatorFactory
    {
        private readonly bool _allowHeapAllocations;
        private readonly int _size;

        public PoolAllocatorFactory(int poolSize, bool allowHeapAllocations = true)
        {
            if (poolSize < 0) throw new ArgumentOutOfRangeException("poolSize", "poolSize can't be negative.");

            _size = poolSize;
            _allowHeapAllocations = allowHeapAllocations;
        }

        public int PoolSize
        {
            get { return _size; }
        }

        IAllocator<T> IAllocatorFactory.CreateAllocator<T>(IAllocatorOperations<T> allocatorOperations)
        {
            return new PoolAllocator<T>(allocatorOperations, _size, _allowHeapAllocations);
        }
    }
}