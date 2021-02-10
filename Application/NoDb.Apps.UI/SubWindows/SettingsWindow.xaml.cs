using NoDb.Business.Service.Services;
using NoDb.Data.Domain.DbModels;
using System.Windows;
using System.Windows.Controls;

namespace NoDb.Apps.UI.SubWindows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        SettingsService _settingsService;

        public SettingsWindow() : this(null)
        {
        }

        public SettingsWindow(SettingsService settingsService)
        {
            _settingsService = settingsService ?? new SettingsService(App.SolutionService.GetSettingsFolder());
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BindSettings();
            if (xSettings.Items.Count > 0) xSettings.SelectedIndex = 0;
        }

        private void XCreateNew_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(xSettingKey.Text)) return;
            _settingsService.New(new NoDbSetting
            {
                SettingsKey = xSettingKey.Text,
                Schema = "dbo",
                ConnectionType = Data.Domain.Enums.NoDbConnectionType.Mssql,
                ConnectionName = "MainConnection",
                DotNetCoreProject = new DotNetCoreProject()
            });
            xSettingKey.Text = "";
            BindSettings();
            xSettings.SelectedIndex = xSettings.Items.Count - 1;
            System.Windows.Forms.MessageBox.Show("Created");
        }

        private void XSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            xSettingDetail.SelectedObject = xSettings.SelectedItem as NoDbSetting;
        }

        private void XSave_Click(object sender, RoutedEventArgs e)
        {
            if (xSettings.SelectedItem == null) return;
            _settingsService.Save(xSettings.SelectedItem as NoDbSetting);
            BindSettings();
            System.Windows.Forms.MessageBox.Show("Saved");
        }

        void BindSettings()
        {
            var selected = xSettings.SelectedIndex;
            xSettings.ItemsSource = null;
            xSettings.ItemsSource = _settingsService.Settings;
            xSettings.SelectedIndex = selected;
        }

    }
}
