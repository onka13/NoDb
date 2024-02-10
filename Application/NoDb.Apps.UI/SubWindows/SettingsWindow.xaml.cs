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
        private readonly NoDbService noDbService;

        public SettingsWindow() : this(null)
        {
        }

        public SettingsWindow(NoDbService noDbService)
        {
            this.noDbService = noDbService ?? App.NoDbService;
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

            noDbService.SettingsService.New(xSettingKey.Text);
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
            noDbService.SettingsService.Save(xSettings.SelectedItem as NoDbSetting);
            BindSettings();
            System.Windows.Forms.MessageBox.Show("Saved");
        }

        void BindSettings()
        {
            var selected = xSettings.SelectedIndex;
            xSettings.ItemsSource = null;
            xSettings.ItemsSource = noDbService.SettingsService.Settings;
            xSettings.SelectedIndex = selected;
        }

    }
}
