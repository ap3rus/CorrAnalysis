using System;

namespace Ap.CorrAnalysis.Common
{
    public class CandleData
    {
        public DateTime Time { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public long Volume { get; set; }
        public string Ticker { get; set; }
    }
}