using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Framework
{
    [Export("CorrelationPublisher", typeof(IDataPublisher<IList<TickerPairCorrelation>>))]
    public class CorrelationPublisher : DataPublisherBase<IList<TickerPairCorrelation>>,
                                        IDataListener<IDictionary<string, HistoricalItem<CandleData>>>
    {
        private readonly IObjectFactory<TickerPairCorrelation> _correlationPairFactory;

        [ImportingConstructor]
        public CorrelationPublisher(
            [Import("CandleDataHistoricalCache")] IDataPublisher<IDictionary<string, HistoricalItem<CandleData>>>
                candleDataHistoricalCache, [Import] IObjectFactory<TickerPairCorrelation> correlationPairFactory)
        {
            _correlationPairFactory = correlationPairFactory;
            candleDataHistoricalCache.AddListener(this);
        }

        void IDataListener<IDictionary<string, HistoricalItem<CandleData>>>.Receive(
            IDictionary<string, HistoricalItem<CandleData>> data)
        {
            string[] symbols;
            var batch = ExtractBatch(data, out symbols);
            var correlation = CalculateCorrelation(batch, symbols);
            NotifyListeners(correlation);
        }

        private IList<TickerPairCorrelation> CalculateCorrelation(double[][] batch, string[] symbols)
        {
            int rowCount = batch.GetLength(0);
            IList<TickerPairCorrelation> result = new List<TickerPairCorrelation>(rowCount * (rowCount - 1)/2);
            int index = 0;
            for (var i = 0; i < rowCount; i++)
            {
                for (var j = i + 1; j < rowCount; j++)
                {
                    var firstBatch = batch[i];
                    var secondBatch = batch[j];
                    if (firstBatch.Length < 3 || secondBatch.Length < 3) continue; // calculating correlation of 1-2 price points is not meaningful, waiting for at least 3 price points

                    double corrCoef = GetPearson(batch[i], batch[j]);
                    TickerPairCorrelation data = _correlationPairFactory.Create();
                    data.CorrCoef = corrCoef;
                    data.Ticker1 = symbols[i];
                    data.Ticker2 = symbols[j];
                    result.Add(data);
                }
            }

            return result;
        }

        public static double GetPearson(double[] x, double[] y)
        {
            int j, n = x.Length;
            double syy = 0.0, sxy = 0.0, sxx = 0.0, ay = 0.0, ax = 0.0;

            for (j = 0; j < n; j++)
            {
                //finds the mean
                ax += x[j];
                ay += y[j];
            }
            ax /= n;
            ay /= n;
            for (j = 0; j < n; j++)
            {
                // compute correlation coefficient
                double xt = x[j] - ax;
                double yt = y[j] - ay;
                sxx += xt*xt;
                syy += yt*yt;
                sxy += xt*yt;
            }
            return sxy/(Math.Sqrt(sxx*syy) + double.Epsilon);
        }


        private double[][] ExtractBatch(IDictionary<string, HistoricalItem<CandleData>> data, out string[] symbols)
        {
            var result = new double[data.Count][];
            var symbols2 = new string[data.Count];

            Parallel.ForEach(data, (stockData, s, rowIndex) =>
                {
                    var historyLength = stockData.Value.Length;
                    double[] row = result[rowIndex] = new double[historyLength];
                    symbols2[rowIndex] = stockData.Key;
                    stockData.Value.For((columnIndex, candleData) =>
                        {
                            row[columnIndex] = candleData.Close;
                        });
                });

            symbols = symbols2;
            return result;
        }
    }
}