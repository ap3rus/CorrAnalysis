using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Framework
{
    [Export("CandleDataSnapshotPublisher", typeof(IDataPublisher<IDictionary<string, CandleData>>))]
    public class CandleDataSnapshotPublisher : DataPublisherBase<IDictionary<string, CandleData>>,
                                               IDataListener<CandleData>,IStartable,IDisposable
    {
        private readonly ConcurrentDictionary<string, CandleData> _snapshot;
        private readonly Timer _timer;
        private readonly TimeSpan _sendingPeriod;

        [ImportingConstructor]
        public CandleDataSnapshotPublisher([Import("CandleDataPublisher")]IDataPublisher<CandleData> candleDataPublisher,
                                           IConfigProvider configProvider)
        {
            _sendingPeriod = TimeSpan.Parse(configProvider.GetValue("CandleDataSnapshotPublisher.SendingPeriod"));
            _snapshot = new ConcurrentDictionary<string, CandleData>();
            _timer = new Timer(SendSnapshot);
            candleDataPublisher.AddListener(this);
        }

        private void SendSnapshot(object state)
        {
            Stop();
            // assuming sending period >> receiving interval
            //  no need to lock collection while notifying listeners,
            //  we'll get good precision anyway
            NotifyListeners(_snapshot);
            Start();
        }

        void IDataListener<CandleData>.Receive(CandleData data)
        {
            data.AddReferenceIfAllocatable();
            _snapshot.AddOrUpdate(data.Ticker, data, (ticker, oldData) =>
                {
                    oldData.ReleaseIfAllocatable();
                    return data;
                });
        }

        public void Start()
        {
            _timer.Change(_sendingPeriod, _sendingPeriod);
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        void IDisposable.Dispose()
        {
            _timer.Dispose();
        }
    }
}