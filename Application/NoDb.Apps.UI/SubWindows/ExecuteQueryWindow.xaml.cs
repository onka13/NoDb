using CoreCommon.Data.EntityFrameworkBase.Components;
using NoDb.Business.Service.Managers;
using NoDb.Business.Service.Services;
using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;
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
    /// Interaction logic for ExecuteQueryWindow.xaml
    /// </summary>
    public partial class ExecuteQueryWindow : Window
    {
        NoDbService _noDbService;
        NoDbConnectionType _connectionType;

        public ExecuteQueryWindow() : this(null)
        {
        }

        public ExecuteQueryWindow(NoDbService noDbService)
        {
            _noDbService = noDbService ?? App.NoDbService;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            xSettings.ItemsSource = _noDbService.SettingsService.Settings;
            if (xSettings.Items.Count > 0) xSettings.SelectedIndex = 0;
        }

        private void XExecute_Click(object sender, RoutedEventArgs e)
        {
            if (xConnections.SelectedItem == null) return;
            var connection = xConnections.SelectedItem as NoDbSettingConnection;
            if (connection.ConnectionType == NoDbConnectionType.None || connection.ConnectionType == NoDbConnectionType.ElasticSearch)
            {
                System.Windows.Forms.MessageBox.Show("Only Sql providers supported for now!");
                return;
            }
            QueryManager.ExecuteQuery(connection, xQuery.Text);
            System.Windows.Forms.MessageBox.Show("Done");
        }

        public void SetQuery(string query, NoDbConnectionType type)
        {
            _connectionType = type;
            xQuery.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinitionByExtension(".sql");
            xQuery.Text = query;
        }

        private void XSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xSettings.SelectedItem == null) return;
            var setting = xSettings.SelectedItem as NoDbSetting;
            xConnections.ItemsSource = setting.Connections;
            if (xConnections.Items.Count > 0) xConnections.SelectedIndex = 0;
        }
    }
}
