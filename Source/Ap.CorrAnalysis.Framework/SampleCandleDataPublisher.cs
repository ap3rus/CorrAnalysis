using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Framework
{
    [Export("CandleDataPublisher", typeof (IDataPublisher<CandleData>))]
    public class SampleCandleDataPublisher : DataPublisherBase<CandleData>, IStartable, IDisposable
    {
        private readonly IObjectFactory<CandleData> _candleDataObjectPool;
        private readonly TimeSpan _interval;
        private readonly Random _random;
        private readonly string[] _symbols;
        private readonly Timer _timer;

        [ImportingConstructor]
        public SampleCandleDataPublisher(IObjectFactory<CandleData> candleDataObjectPool, IConfigProvider configProvider)
        {
            _random = new Random();
            _symbols =
                Enumerable.Range(1, int.Parse(configProvider.GetValue("SampleCandleDataPublisher.TickersCount")))
                          .Select(i => "TEST#" + i)
                          .ToArray();
            _candleDataObjectPool = candleDataObjectPool;
            _interval = TimeSpan.Parse(configProvider.GetValue("SampleCandleDataPublisher.GenerationInterval"));
            _timer = new Timer(GenerateNewQuote);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Start()
        {
            _timer.Change(_interval, _interval);
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void GenerateNewQuote(object state)
        {
            foreach (string symbol in _symbols)
            {
                CandleData quote = _candleDataObjectPool.Create();
                quote.Ticker = symbol;
                quote.Time = DateTime.Now;
                quote.Close = 100*_random.NextDouble();
                NotifyListeners(quote);
                quote.ReleaseIfAllocatable();
            }
        }

        ~SampleCandleDataPublisher()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            _timer.Dispose();
        }
    }
}