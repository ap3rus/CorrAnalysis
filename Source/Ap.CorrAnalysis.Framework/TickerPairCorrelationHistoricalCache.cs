using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Framework
{
    [Export("TickerPairCorrelationHistoricalCache", typeof(IDataPublisher<IDictionary<TickerPair, HistoricalItem<TickerPairCorrelation>>>))]
    public class TickerPairCorrelationHistoricalCache :
        DataPublisherBase<IDictionary<TickerPair, HistoricalItem<TickerPairCorrelation>>>,
        IDataListener<IList<TickerPairCorrelation>>
    {
        private readonly ConcurrentDictionary<TickerPair, HistoricalItem<TickerPairCorrelation>> _historicalCache;
        private readonly int _maxHistoryLength;
        private readonly int _maxStorageSize;

        [ImportingConstructor]
        public TickerPairCorrelationHistoricalCache(
            [Import("CorrelationPublisher")] IDataPublisher<IList<TickerPairCorrelation>> correlationPublisher,
            IConfigProvider configProvider)
        {
            _historicalCache = new ConcurrentDictionary<TickerPair, HistoricalItem<TickerPairCorrelation>>();
            _maxHistoryLength = int.Parse(configProvider.GetValue("TickerPairCorrelationHistoricalCache.MaxHistoryLength"));
            _maxStorageSize = int.Parse(configProvider.GetValue("TickerPairCorrelationHistoricalCache.MaxStorageSize"));
            correlationPublisher.AddListener(this);
        }

        void IDataListener<IList<TickerPairCorrelation>>.Receive(IList<TickerPairCorrelation> data)
        {
            Parallel.ForEach(data,
                             p =>
                             _historicalCache.AddOrUpdate(p,
                                                          s =>
                                                          new HistoricalItem<TickerPairCorrelation>(_maxHistoryLength,
                                                                                                    _maxStorageSize), (s, h) =>
                                                                                                        {
                                                                                                            h.Add(p);
                                                                                                            return h;
                                                                                                        }));
            NotifyListeners(_historicalCache);
        }
    }
}