
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Framework
{
    [Export("TopCorrelationPublisher", typeof(IDataPublisher<IList<TickerPairCorrelation>>))]
    public class TopCorrelationPublisher : DataPublisherBase<IList<TickerPairCorrelation>>,
                                           IDataListener<IList<TickerPairCorrelation>>
    {
        private readonly int _maxCount;

        [ImportingConstructor]
        public TopCorrelationPublisher(
            [Import("CorrelationPublisher")] IDataPublisher<IList<TickerPairCorrelation>> correlationPublisher,
            IConfigProvider configProvider)
        {
            _maxCount = int.Parse(configProvider.GetValue("TopCorrelationPublisher.MaxCount"));
            correlationPublisher.AddListener(this);
        }

        void IDataListener<IList<TickerPairCorrelation>>.Receive(IList<TickerPairCorrelation> data)
        {
            var sw = new Stopwatch();
            sw.Start();
            NotifyListeners(GetTopNCorrelations(data, _maxCount));
            sw.Stop();
            Trace.TraceInformation("Top {0} correlations calculated by {1}", _maxCount, sw.Elapsed);
        }

        private static IList<TickerPairCorrelation> GetTopNCorrelations(IEnumerable<TickerPairCorrelation> data, int maxCount)
        {
            var topPairs = new List<TickerPairCorrelation>();
            foreach (var item in data)
            {
                int indexToInsert = -1;
                for (var i = topPairs.Count - 1; i >= 0; i--)
                {
                    if (item.CorrCoef > topPairs[i].CorrCoef)
                    {
                        indexToInsert = i;
                    }
                    else
                    {
                        break;
                    }
                }

                if (indexToInsert < 0)
                {
                    if (topPairs.Count < maxCount)
                    {
                        topPairs.Add(item);
                    }

                    continue;
                }

                topPairs.Insert(indexToInsert, item);
                if (topPairs.Count > maxCount)
                {
                    topPairs.RemoveAt(topPairs.Count - 1);
                }
            }

            return topPairs;
        }
    }
}