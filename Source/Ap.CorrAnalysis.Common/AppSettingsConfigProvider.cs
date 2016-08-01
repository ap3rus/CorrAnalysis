using System.ComponentModel.Composition;
using System.Configuration;

namespace Ap.CorrAnalysis.Common
{
    [Export(typeof (IConfigProvider))]
    public class AppSettingsConfigProvider : IConfigProvider
    {
        public string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}