using System.ComponentModel.Composition;
using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Allocation
{
    [Export(typeof (IObjectFactory<CandleData>))]
    public class CandleDataObjectPool : IObjectFactory<CandleData>
    {
        private readonly IAllocator<AllocatableCandleData> _allocator;
        private readonly IAllocatorFactory _allocatorFactory;

        [ImportingConstructor]
        public CandleDataObjectPool([Import] IConfigProvider configProvider)
        {
            _allocatorFactory =
                new PoolAllocatorFactory(poolSize: int.Parse(configProvider.GetValue("Allocation.PoolSize")));
            _allocator = _allocatorFactory.CreateAllocator(new CandleDataAllocatorType());
        }

        CandleData IObjectFactory<CandleData>.Create()
        {
            return _allocator.Allocate();
        }
    }
}