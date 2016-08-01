namespace Ap.CorrAnalysis.Common
{
    public static class Extensions
    {
        public static void AddReferenceIfAllocatable<T>(this T obj)
        {
            var allocatable = obj as IAllocatable;
            if (allocatable != null) allocatable.AddReference();
        }

        public static void ReleaseIfAllocatable<T>(this T obj)
        {
            var allocatable = obj as IAllocatable;
            if (allocatable != null) allocatable.Release();
        }
    }
}