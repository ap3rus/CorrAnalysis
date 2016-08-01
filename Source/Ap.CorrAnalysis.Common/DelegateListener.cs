using System;

namespace Ap.CorrAnalysis.Common
{
    public class DelegateListener<T> : IDataListener<T>
    {
        private readonly Action<T> _handler;

        public DelegateListener(Action<T> handler)
        {
            if (handler == null) throw new ArgumentNullException("handler");

            _handler = handler;
        }

        public void Receive(T data)
        {
            _handler(data);
        }
    }
}