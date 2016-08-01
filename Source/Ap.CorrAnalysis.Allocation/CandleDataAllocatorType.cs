using System;

namespace Ap.CorrAnalysis.Allocation
{
    public class CandleDataAllocatorType : IAllocatorOperations<AllocatableCandleData>
    {
        AllocatableCandleData IAllocatorOperations<AllocatableCandleData>.CreateNew(
            AllocatableHelper<AllocatableCandleData> allocatableHelper)
        {
            return new AllocatableCandleData(allocatableHelper);
        }

        void IAllocatorOperations<AllocatableCandleData>.ResetObject(AllocatableCandleData obj)
        {
            obj.Close = 0;
            obj.Open = 0;
            obj.High = 0;
            obj.Low = 0;
            obj.Time = default(DateTime);
            obj.Volume = 0;
            obj.Ticker = null;
        }

        void IAllocatorOperations<AllocatableCandleData>.SetReferenceCountToOne(AllocatableCandleData obj)
        {
            obj.SetReferenceCountToOne();
        }
    }
}