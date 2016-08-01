namespace Ap.CorrAnalysis.Common
{
    public interface IAllocatable
    {
        void AddReference();
        void Release();
    }
}