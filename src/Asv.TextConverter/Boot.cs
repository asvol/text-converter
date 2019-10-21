using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using NLog;
using LogManager = NLog.LogManager;


namespace Asv.TextConverter
{
    public class Boot : BootstrapperBase, IDisposable
    {
        private CompositionContainer _container;
        
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();


        public Boot()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            _logger.Info($"Startup with args: {string.Join(" ", e.Args)}");
            DisplayRootViewFor<IShell>();
        }

        protected override void Configure()
        {

            //An aggregate catalog that combines multiple catalogs  
            var catalog = new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>());

            //Create the CompositionContainer with the parts in the catalog  
            _container = new CompositionContainer(catalog);

            var batch = new CompositionBatch();
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(_container);

            if (Execute.InDesignMode)
            {
                batch.AddExportedValue<IConfiguration>(new JsonOneFileConfiguration(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "config.json")));
            }
            else
            {
                batch.AddExportedValue<IConfiguration>(new JsonOneFileConfiguration("config.json"));
            }

            _container.Compose(batch);

            var defaultLocator = ViewLocator.LocateTypeForModelType;
            ViewLocator.LocateTypeForModelType = (modelType, displayLocation, context) =>
            {
                var viewType = defaultLocator(modelType, displayLocation, context);
                while (viewType == null && modelType != typeof(object))
                {
                    modelType = modelType.BaseType;
                    viewType = defaultLocator(modelType, displayLocation, context);
                }
                return viewType;
            };

        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = _container.GetExportedValues<object>(contract);

            if (exports.Any())
                return exports.First();

            throw new Exception(string.Format("Could not locate instance {0}", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            yield return typeof(IShell).Assembly;
        }

        protected override void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }

        public void Dispose()
        {
            _container?.Dispose();
        }
    }
}