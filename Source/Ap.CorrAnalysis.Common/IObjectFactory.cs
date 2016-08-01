namespace Ap.CorrAnalysis.Common
{
    public interface IObjectFactory<out T>
    {
        T Create();
    }
}