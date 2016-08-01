namespace Ap.CorrAnalysis.Common
{
    public interface IDataListener<in T>
    {
        void Receive(T data);
    }
}