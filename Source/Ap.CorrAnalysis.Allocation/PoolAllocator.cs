using System.Collections.Concurrent;
using System.Diagnostics;
using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Allocation
{
    public class PoolAllocator<T> : IAllocator<T>, IReleaser<T> where T : IAllocatable
    {
        private readonly AllocatableHelper<T> _allocatableHelper;
        private readonly IAllocatorOperations<T> _allocatorOperations;
        private readonly bool _allowOnHeapWhenEmpty;
        private readonly ConcurrentBag<T> _collection;
        private readonly AllocatableHelper<T> _heapAllocatorHelper;

        public PoolAllocator(IAllocatorOperations<T> allocatorOperations, int poolSize, bool allowOnHeapWhenEmpty)
        {
            _allocatorOperations = allocatorOperations;
            _allowOnHeapWhenEmpty = allowOnHeapWhenEmpty;
            _allocatableHelper = new AllocatableHelper<T>(this, this);
            _heapAllocatorHelper = new AllocatableHelper<T>(this, NullReleaser<T>.Instance);
            _collection = new ConcurrentBag<T>();
            Trace.TraceInformation("PoolAllocator is allocating {0} instances of type {1}...", poolSize, typeof(T).FullName);
            for (int i = 0; i < poolSize; i++)
            {
                _collection.Add(_allocatorOperations.CreateNew(_allocatableHelper));
            }

            Trace.TraceInformation("Done");
        }

        T IAllocator<T>.Allocate()
        {
            T obj;
            if (!_collection.TryTake(out obj))
            {
                if (!_allowOnHeapWhenEmpty) return default(T);
                obj = _allocatorOperations.CreateNew(_heapAllocatorHelper);
            }

            _allocatorOperations.SetReferenceCountToOne(obj);
            return obj;
        }

        void IReleaser<T>.Release(T obj)
        {
            _allocatorOperations.ResetObject(obj);
            _collection.Add(obj);
        }
    }
}