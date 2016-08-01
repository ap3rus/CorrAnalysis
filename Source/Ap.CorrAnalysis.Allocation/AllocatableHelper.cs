using System;
using System.Threading;
using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Allocation
{
    public class AllocatableHelper<T> where T : IAllocatable
    {
        private readonly IAllocator<T> _allocator;
        private readonly IReleaser<T> _releaser;

        public AllocatableHelper(IAllocator<T> allocator, IReleaser<T> releaser)
        {
            _allocator = allocator;
            _releaser = releaser;
        }

        public IReleaser<T> Releaser
        {
            get { return _releaser; }
        }

        public IAllocator<T> Allocator
        {
            get { return _allocator; }
        }

        public void AddReference(ref int refCount)
        {
            int comparand;
            do
            {
                comparand = refCount;
                if (comparand == 0)
                {
                    throw new InvalidOperationException(
                        "Reference count = 0, either object was not allocated or has already been released.");
                }
            } while (Interlocked.CompareExchange(ref refCount, comparand + 1, comparand) != comparand);
        }

        public void Release(object item, ref int refCount)
        {
            int comparand;
            do
            {
                comparand = refCount;
                if (comparand == 0)
                {
                    throw new InvalidOperationException("Reference count was already 0.");
                }
            } while (Interlocked.CompareExchange(ref refCount, comparand - 1, comparand) != comparand);
            if (comparand != 1) return;
            _releaser.Release((T) item);
        }
    }
}