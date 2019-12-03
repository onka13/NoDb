using NoDb.Business.Service.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NoDb.Apps.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string Folder { get; set; }
        public static bool OpenSettings { get; set; }
        public static NoDbService NoDbService { get; set; }

        public App()
        {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var args = e.Args;
            if (args != null)
            {
                for (int i = 0; i < args.Length; i += 2)
                {
                    if (args.Length < i + 1) continue;
                    if (args[i] == "-folder")
                    {
                        Folder = args[i + 1];
                    } 
                    else if (args[i] == "-openSettings")
                    {
                        OpenSettings = args[i + 1] == "true";
                    }
                }
            }

            if (OpenSettings)
            {
                NoDbService = new NoDbService(Folder);
                StartupUri = new Uri("SubWindows/SettingsWindow.xaml", UriKind.Relative);
                return;
            }

            StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            string errorMessage = string.Format("Error: {0}", e.Exception.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            // TODO: log
            e.Handled = true;
        }
    }
}
