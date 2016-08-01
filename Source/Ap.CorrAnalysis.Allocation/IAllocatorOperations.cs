using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Allocation
{
    public interface IAllocatorOperations<T> where T : IAllocatable
    {
        T CreateNew(AllocatableHelper<T> allocatableHelper);
        void ResetObject(T obj);
        void SetReferenceCountToOne(T obj);
    }
}