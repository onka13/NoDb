using CoreCommon.Data.EntityFrameworkBase.Models;
using NoDb.Business.Service.Managers;
using NoDb.Business.Service.Services;
using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NoDb.Apps.UI.SubWindows
{
    /// <summary>
    /// Interaction logic for ImportFromSqlWindow.xaml
    /// </summary>
    public partial class ImportFromSqlWindow : Window
    {
        NoDbService _noDbService;
        List<InformationSchemaTable> dbTables;
        List<InformationSchemaColumn> dbColumns;

        public ImportFromSqlWindow() : this(null)
        {
        }

        public ImportFromSqlWindow(NoDbService noDbService)
        {
            _noDbService = noDbService ?? App.NoDbService;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            xSettings.ItemsSource = new SettingsService(App.SolutionService.GetSettingsFolder()).Settings;
            if (xSettings.Items.Count > 0) xSettings.SelectedIndex = 0;
        }

        private void XFetchTables_Click(object sender, RoutedEventArgs e)
        {
            if (xConnections.SelectedItem == null) return;
            var connection = xConnections.SelectedItem as NoDbSettingConnection;
            if (connection.ConnectionType == NoDbConnectionType.None || connection.ConnectionType == NoDbConnectionType.ElasticSearch)
            {
                System.Windows.Forms.MessageBox.Show("Only Sql providers supported for now!");
                return;
            }

            dbTables = QueryManager.GetTablesInformation(connection);
            dbColumns = QueryManager.GetColumnsInformation(connection);

            xTables.ItemsSource = null;
            xTables.ItemsSource = dbTables.OrderBy(x => x.TableSchema).ThenBy(x => x.TableName).ToList();
        }

        private void XSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xSettings.SelectedItem == null)
            {
                xConnections.ItemsSource = null;
                return;
            }

            var setting = xSettings.SelectedItem as NoDbSetting;
            xConnections.ItemsSource = setting.Connections;
            if (xConnections.Items.Count > 0) xConnections.SelectedIndex = 0;
        }

        private void XTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xTables.SelectedItem == null)
            {
                return;
            }
            var table = xTables.SelectedItem as InformationSchemaTable;
            //var connection = xConnections.SelectedItem as NoDbSettingConnection;
            //var columns = QueryManager.GetColumnsInformation(connection, table.TableName);

            xColumns.ItemsSource = null;
            xColumns.ItemsSource = dbColumns.Where(x => x.TableName == table.TableName).OrderBy(x => x.ColumnName).ToList();
        }

        private void XSync_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tables = xTables.SelectedItems.Cast<InformationSchemaTable>().ToList();
                if (tables.Count == 0)
                {
                    tables = dbTables;
                }

                if (System.Windows.Forms.MessageBox.Show($"SYNC will be run for {tables.Count} table(s)") != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }

                var setting = xSettings.SelectedItem as NoDbSetting;
                var connection = xConnections.SelectedItem as NoDbSettingConnection;
                new ImportService(_noDbService).SyncFromDb(setting, connection.ConnectionType, tables, dbColumns);
                System.Windows.Forms.MessageBox.Show("Done!");
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}