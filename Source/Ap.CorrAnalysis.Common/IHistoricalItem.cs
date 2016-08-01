using System;

namespace Ap.CorrAnalysis.Common
{
    public interface IHistoricalItem<T>
    {
        T Latest { get; }
        void Add(T item);
        void For(Action<int, T> iterator);
    }
}