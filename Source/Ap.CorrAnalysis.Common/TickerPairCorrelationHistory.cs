using System;
using System.Threading;

namespace Ap.CorrAnalysis.Common
{
    public class TickerPairCorrelationHistory: IHistoricalItem<double>
    {
        public TickerPair TickerPair { get; private set; }

        public double CurrentCorrelation
        {
            get { return _correlationHistory.Latest; }
        }

        private readonly HistoricalItem<double> _correlationHistory;

        public TickerPairCorrelationHistory(int maxHistoryLength, int maxStorageSize, TickerPair tickerPair)
        {
            _correlationHistory = new HistoricalItem<double>(maxHistoryLength, maxStorageSize);
            TickerPair = tickerPair;
        }

        public void Add(double correlation)
        {
            _correlationHistory.Add(correlation);
        }

        public void For(Action<int, double> iterator)
        {
            _correlationHistory.For(iterator);
        }

        double IHistoricalItem<double>.Latest
        {
            get { return CurrentCorrelation; }
        }
    }
}