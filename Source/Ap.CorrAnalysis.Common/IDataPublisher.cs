namespace Ap.CorrAnalysis.Common
{
    public interface IDataPublisher<out T>
    {
        void AddListener(IDataListener<T> listener);
        void RemoveListener(IDataListener<T> listener);
    }
}