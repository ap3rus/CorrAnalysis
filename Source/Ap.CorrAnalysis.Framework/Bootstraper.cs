using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Framework
{
    [Export(typeof (IBootstraper))]
    public class Bootstraper : IBootstraper
    {
        [Import("CandleDataPublisher")]private IDataPublisher<CandleData> _candleDataPublisher;
        [Import("CandleDataSnapshotPublisher")]private IDataPublisher<IDictionary<string, CandleData>> _candleDataSnapshotPublisher;
        [Import("CandleDataHistoricalCache")]private IDataPublisher<IDictionary<string, HistoricalItem<CandleData>>> _candleDataHistoricalCache;
        [Import("CorrelationPublisher")]private IDataPublisher<IList<TickerPairCorrelation>> _correlationPublisher;
        [Import("TickerPairCorrelationHistoricalCache")]private IDataPublisher<IDictionary<TickerPair, HistoricalItem<TickerPairCorrelation>>> _tickerPairCorrelationHistoricalCache;
        [Import("TopCorrelationPublisher")]private IDataPublisher<IList<TickerPairCorrelation>> _topCorrelationPublisher;
        [Import("CorrelationDivergenceDetector")]private IDataPublisher<HistoricalItem<TickerPairCorrelation>> _correlationDivergenceDetector;

        public void Start()
        {
            Start(_candleDataPublisher);
            Start(_candleDataSnapshotPublisher);
            Trace.TraceInformation("Started.");
        }

        public void Stop()
        {
            Stop(_candleDataSnapshotPublisher);
            Stop(_candleDataPublisher);
            Trace.TraceInformation("Stopped.");
        }

        private static void Start<T>(T obj)
        {
            var startable = obj as IStartable;
            if (startable != null)
                startable.Start();
        }

        private static void Stop<T>(T obj)
        {
            var startable = obj as IStartable;
            if (startable != null)
                startable.Stop();
        }
    }
}