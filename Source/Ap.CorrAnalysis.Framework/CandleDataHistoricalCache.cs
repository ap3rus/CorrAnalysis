using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Framework
{
    [Export("CandleDataHistoricalCache", typeof(IDataPublisher<IDictionary<string, HistoricalItem<CandleData>>>))]
    public class CandleDataHistoricalCache : DataPublisherBase<IDictionary<string, HistoricalItem<CandleData>>>,
                                             IDataListener<IDictionary<string, CandleData>>
    {
        private readonly ConcurrentDictionary<string, HistoricalItem<CandleData>> _historicalCache;
        private readonly int _maxHistoryLength;
        private readonly int _maxStorageSize;

        [ImportingConstructor]
        public CandleDataHistoricalCache(
            [Import("CandleDataSnapshotPublisher")] IDataPublisher<IDictionary<string, CandleData>>
                candleDataSnapshotPublisher, IConfigProvider configProvider)
        {
            _historicalCache = new ConcurrentDictionary<string, HistoricalItem<CandleData>>();
            _maxHistoryLength = int.Parse(configProvider.GetValue("CandleDataHistoricalCache.MaxHistoryLength"));
            _maxStorageSize = int.Parse(configProvider.GetValue("CandleDataHistoricalCache.MaxStorageSize"));
            candleDataSnapshotPublisher.AddListener(this);
        }

        void IDataListener<IDictionary<string, CandleData>>.Receive(IDictionary<string, CandleData> data)
        {
            Parallel.ForEach(data,
                             p =>
                             _historicalCache.AddOrUpdate(p.Key,
                                                          s =>
                                                          new HistoricalItem<CandleData>(_maxHistoryLength,
                                                                                         _maxStorageSize), (s, h) =>
                                                                                             {
                                                                                                 h.Add(p.Value);
                                                                                                 return h;
                                                                                             }));
            NotifyListeners(_historicalCache);
        }
    }
}