namespace Ap.CorrAnalysis.Allocation
{
    public class NullReleaser<T> : IReleaser<T>
    {
        public static readonly NullReleaser<T> Instance = new NullReleaser<T>();

        private NullReleaser()
        {
        }

        void IReleaser<T>.Release(T obj)
        {
        }
    }
}