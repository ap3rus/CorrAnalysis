namespace Ap.CorrAnalysis.Allocation
{
    public class CorrelationPairAllocatorType : IAllocatorOperations<AllocatableTickerPairCorrelation>
    {
        AllocatableTickerPairCorrelation IAllocatorOperations<AllocatableTickerPairCorrelation>.CreateNew(
            AllocatableHelper<AllocatableTickerPairCorrelation> allocatableHelper)
        {
            return new AllocatableTickerPairCorrelation(allocatableHelper);
        }

        void IAllocatorOperations<AllocatableTickerPairCorrelation>.ResetObject(AllocatableTickerPairCorrelation obj)
        {
            obj.CorrCoef = 0;
            obj.Ticker1 = null;
            obj.Ticker2 = null;
        }

        void IAllocatorOperations<AllocatableTickerPairCorrelation>.SetReferenceCountToOne(AllocatableTickerPairCorrelation obj)
        {
            obj.SetReferenceCountToOne();
        }
    }
}