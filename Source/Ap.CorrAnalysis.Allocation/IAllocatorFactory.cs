using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Allocation
{
    public interface IAllocatorFactory
    {
        IAllocator<T> CreateAllocator<T>(IAllocatorOperations<T> allocatorOperations) where T : class, IAllocatable;
    }
}