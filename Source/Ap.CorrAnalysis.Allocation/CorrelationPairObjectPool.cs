using System.ComponentModel.Composition;
using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Allocation
{
    [Export(typeof (IObjectFactory<TickerPairCorrelation>))]
    public class CorrelationPairObjectPool : IObjectFactory<TickerPairCorrelation>
    {
        private readonly IAllocator<AllocatableTickerPairCorrelation> _allocator;
        private readonly IAllocatorFactory _allocatorFactory;

        [ImportingConstructor]
        public CorrelationPairObjectPool([Import] IConfigProvider configProvider)
        {
            _allocatorFactory =
                new PoolAllocatorFactory(poolSize: int.Parse(configProvider.GetValue("Allocation.PoolSize")));
            _allocator = _allocatorFactory.CreateAllocator(new CorrelationPairAllocatorType());
        }

        TickerPairCorrelation IObjectFactory<TickerPairCorrelation>.Create()
        {
            return _allocator.Allocate();
        }
    }
}