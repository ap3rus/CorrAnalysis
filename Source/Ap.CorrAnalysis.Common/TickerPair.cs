using System;

namespace Ap.CorrAnalysis.Common
{
    public class TickerPair
    {
        public string Ticker1 { get; set; }
        public string Ticker2 { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            var another = obj as TickerPair;
            if (another == null) return false;

            return Ticker1 == another.Ticker1 && Ticker2 == another.Ticker2;
        }

        public override int GetHashCode()
        {
            return (Ticker1 ?? string.Empty).GetHashCode() ^ (Ticker2 ?? string.Empty).GetHashCode();
        }
    }
}