using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NLog;

namespace Asv.TextConverter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private Boot _boot;

        public App()
        {
            Current.DispatcherUnhandledException += (s, a) =>
            {
                _logger.Fatal(a.Exception, "App unhandled exception:{0}", a.Exception.Message);
                a.Handled =
                    MessageBox.Show(a.Exception.Message, "Unhandled error", MessageBoxButton.OK,
                        MessageBoxImage.Error) == MessageBoxResult.OK;
            };

            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;


            _boot = new Boot();

        }
    }
}
