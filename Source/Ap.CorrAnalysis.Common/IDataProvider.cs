using System.Collections.Generic;

namespace Ap.CorrAnalysis.Common
{
    public interface IDataProvider<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        object SyncRoot { get; }
        TValue Get(TKey key);
    }
}