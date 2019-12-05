using NoDb.Business.Service.Services;
using NoDb.Data.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NoDb.Apps.UI.SubWindows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        NoDbService _noDbService;

        public SettingsWindow() : this(null)
        {
        }

        public SettingsWindow(NoDbService noDbService)
        {
            _noDbService = noDbService ?? App.NoDbService;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BindSettings();
            if (xSettings.Items.Count > 0) xSettings.SelectedIndex = 0;
        }

        private void XCreateNew_Click(object sender, RoutedEventArgs e)
        {
            _noDbService.SettingsService.New(new NoDbSetting
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
            _noDbService.SettingsService.Save(xSettings.SelectedItem as NoDbSetting);
            BindSettings();
            System.Windows.Forms.MessageBox.Show("Saved");
        }

        void BindSettings()
        {
            var selected = xSettings.SelectedIndex;
            xSettings.ItemsSource = null;
            xSettings.ItemsSource = _noDbService.SettingsService.Settings;
            xSettings.SelectedIndex = selected;
        }

    }
}
