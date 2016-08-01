using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Allocation
{
    public interface IAllocator<out T> where T : IAllocatable
    {
        T Allocate();
    }
}