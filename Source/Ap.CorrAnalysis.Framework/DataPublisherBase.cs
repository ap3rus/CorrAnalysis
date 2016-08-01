using System.Collections.Generic;
using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.Framework
{
    public class DataPublisherBase<T> : IDataPublisher<T>
    {
        private readonly HashSet<IDataListener<T>> _listeners;

        public DataPublisherBase()
        {
            _listeners = new HashSet<IDataListener<T>>();
        }

        public void AddListener(IDataListener<T> listener)
        {
            _listeners.Add(listener);
        }

        public void RemoveListener(IDataListener<T> listener)
        {
            _listeners.Remove(listener);
        }

        protected void NotifyListeners(T item)
        {
            foreach (var listener in _listeners)
            {
                listener.Receive(item);
            }
        }
    }
}