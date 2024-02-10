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
        public static NoDbService NoDbService { get; private set; }

        public App()
        {
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

        public static void InitNoDbService(string solution)
        {
            if (string.IsNullOrEmpty(solution))
            {
                return;
            }

            NoDbService = new NoDbService(solution);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string solution = null; 
            bool openSettings = false, openSolutionSettings = false;

            var args = e.Args;
            if (args != null)
            {
                for (int i = 0; i < args.Length; i += 2)
                {
                    if (args.Length < i + 1) continue;
                    if (args[i] == "-solution")
                    {
                        solution = args[i + 1];
                    }
                    else if (args[i] == "-openSettings")
                    {
                        openSettings = args[i + 1] == "true";
                    }
                    else if (args[i] == "-openSolutionSettings")
                    {
                        openSolutionSettings = args[i + 1] == "true";
                    }
                }
            }

            InitNoDbService(solution);

            if (openSettings)
            {
                StartupUri = new Uri("SubWindows/SettingsWindow.xaml", UriKind.Relative);
                return;
            }

            if (openSolutionSettings)
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
