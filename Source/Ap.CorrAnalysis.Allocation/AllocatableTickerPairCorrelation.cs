using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Allocation
{
    public class AllocatableTickerPairCorrelation : TickerPairCorrelation, IAllocatable
    {
        private readonly AllocatableHelper<AllocatableTickerPairCorrelation> _allocatableHelper;
        private int _refCount;

        public AllocatableTickerPairCorrelation(AllocatableHelper<AllocatableTickerPairCorrelation> allocatableHelper)
        {
            _allocatableHelper = allocatableHelper;
        }

        public void AddReference()
        {
            if (_allocatableHelper != null) _allocatableHelper.AddReference(ref _refCount);
        }

        public void Release()
        {
            if (_allocatableHelper != null) _allocatableHelper.Release(this, ref _refCount);
        }

        internal void SetReferenceCountToOne()
        {
            _refCount = 1;
        }
    }
}