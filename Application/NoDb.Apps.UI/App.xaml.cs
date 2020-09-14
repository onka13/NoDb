using NoDb.Business.Service.Services;
using System;
using System.Windows;

namespace NoDb.Apps.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string Folder { get; set; }
        public static string SolutionFolder { get; set; }
        public static string InitialProject { get; set; }
        public static bool OpenSettings { get; set; }
        public static bool OpenSolutionSettings { get; set; }
        public static NoDbSolutionService SolutionService { get; set; }
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
                    else if (args[i] == "-solution")
                    {
                        SolutionFolder = args[i + 1];
                    }
                    else if (args[i] == "-project")
                    {
                        InitialProject = args[i + 1];
                    }
                    else if (args[i] == "-openSettings")
                    {
                        OpenSettings = args[i + 1] == "true";
                    }
                    else if (args[i] == "-openSolutionSettings")
                    {
                        OpenSolutionSettings = args[i + 1] == "true";
                    }
                }
            }
            if (SolutionFolder != null)
            {
                SolutionService = new NoDbSolutionService(SolutionFolder);
            }

            if (Folder != null)
            {
                NoDbService = new NoDbService(Folder);
            }

            if (OpenSettings)
            {
                StartupUri = new Uri("SubWindows/SettingsWindow.xaml", UriKind.Relative);
                return;
            }

            if (OpenSolutionSettings)
            {
                StartupUri = new Uri("SubWindows/SolutionWindow.xaml", UriKind.Relative);
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
