using NoDb.Business.Service.Managers;
using NoDb.Business.Service.Services;
using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;
using System.Windows;
using System.Windows.Controls;

namespace NoDb.Apps.UI.SubWindows
{
    /// <summary>
    /// Interaction logic for ExecuteQueryWindow.xaml
    /// </summary>
    public partial class ExecuteQueryWindow : Window
    {
        private readonly NoDbService noDbService;

        public ExecuteQueryWindow() : this(null)
        {
        }

        public ExecuteQueryWindow(NoDbService noDbService)
        {
            this.noDbService = noDbService ?? App.NoDbService;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            xSettings.ItemsSource = noDbService.SettingsService.Settings;
            if (xSettings.Items.Count > 0) xSettings.SelectedIndex = 0;
            xQueryHistory.ItemsSource = noDbService.QueryHistoryService.HistoryFileNames;
            if (xQueryHistory.Items.Count > 0) xQueryHistory.SelectedIndex = 0;
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

        private void XAppendQuery_Click(object sender, RoutedEventArgs e)
        {
            if (xQueryHistory.SelectedItem == null) return;
            noDbService.QueryHistoryService.Save(xQueryHistory.SelectedItem.ToString(), xQuery.Text, true); 
            System.Windows.Forms.MessageBox.Show("Saved!");
        }

        private void XOpenQueryHistory_Click(object sender, RoutedEventArgs e)
        {
            var window = new QueryHistoryWindow();
            window.Show();
        }
    }
}
