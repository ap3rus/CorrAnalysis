using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using Ap.CorrAnalysis.Common;

namespace Ap.CorrAnalysis.ConsoleHost
{
    internal class Host
    {
        private readonly CompositionContainer _container;
        [Import] private IBootstraper _bootstraper;
        [Import] private IObjectFactory<CandleData> _pool;

        public Host()
        {
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(new FileInfo(typeof (Host).Assembly.Location).DirectoryName));

            _container = new CompositionContainer(catalog);
            try
            {
                _container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }

        public IBootstraper Bootstraper
        {
            get { return _bootstraper; }
        }
    }
}