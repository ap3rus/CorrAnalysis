namespace Ap.CorrAnalysis.Allocation
{
    public interface IReleaser<in T>
    {
        void Release(T obj);
    }
}