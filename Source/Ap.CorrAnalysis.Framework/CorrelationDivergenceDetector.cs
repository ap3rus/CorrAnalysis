using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Framework
{
    [Export("CorrelationDivergenceDetector", typeof(IDataPublisher<HistoricalItem<TickerPairCorrelation>>))]
    public class CorrelationDivergenceDetector :DataPublisherBase<HistoricalItem<TickerPairCorrelation>>,
                                                IDataListener<IDictionary<TickerPair, HistoricalItem<TickerPairCorrelation>>>
    {
        [ImportingConstructor]
        public CorrelationDivergenceDetector([Import("TickerPairCorrelationHistoricalCache")]IDataPublisher<IDictionary<TickerPair, HistoricalItem<TickerPairCorrelation>>> tickerPairCorrelationHistoricalCache)
        {
            tickerPairCorrelationHistoricalCache.AddListener(this);
        }

        void IDataListener<IDictionary<TickerPair, HistoricalItem<TickerPairCorrelation>>>.Receive(IDictionary<TickerPair, HistoricalItem<TickerPairCorrelation>> data)
        {
            foreach (var item in data)
            {
                if (item.Value.Length > 2) // waiting for at least 3 items in a history
                {
                    if (Math.Abs(item.Value[0].CorrCoef - item.Value[1].CorrCoef) >
                        Math.Abs(GetStandardDeviation(item.Value)*2))
                    {
                        NotifyListeners(item.Value);
                    }
                }
            }
        }

        private double GetStandardDeviation(HistoricalItem<TickerPairCorrelation> correlations)
        {
            double mean = 0;
            int count = 0;
            double sumOfDerivation = 0;
            correlations.For((index, pair) =>
            {
                count++;
                mean += pair.CorrCoef;
                sumOfDerivation += (pair.CorrCoef) * (pair.CorrCoef);
            });
            mean /= count;
            correlations.For((index, pair) =>
                {
                });

            double sumOfDerivationAverage = sumOfDerivation / count;
            return Math.Sqrt(sumOfDerivationAverage - (mean * mean));
        }
    }
}