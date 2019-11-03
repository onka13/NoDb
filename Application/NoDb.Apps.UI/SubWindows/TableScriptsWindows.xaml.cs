using NoDb.Business.Service.Managers;
using NoDb.Business.Service.Services;
using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace NoDb.Apps.UI.SubWindows
{
    /// <summary>
    /// Interaction logic for TableScriptsWİndows.xaml
    /// </summary>
    public partial class TableScriptsWindow : Window
    {
        NoDbService _noDbService;
        NoDbConnectionType _noDbConnectionType = NoDbConnectionType.Mssql;

        public TableScriptsWindow(NoDbService noDbService)
        {
            InitializeComponent();
            _noDbService = noDbService;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            xTables.ItemsSource = _noDbService.TableService.Tables.Where(x => !x.Detail.Ignored).ToList();
            xQueryType.ItemsSource = Enum.GetNames(typeof(NoDbConnectionType));
            xQueryType.SelectedItem = _noDbConnectionType.ToString();
            xQuery.Document = new FlowDocument();
        }

        private void XTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateQuery();
        }

        private void XQueryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _noDbConnectionType = (NoDbConnectionType)Enum.Parse(typeof(NoDbConnectionType), xQueryType.SelectedItem.ToString());
            UpdateQuery();
        }

        private void XDropBefore_Click(object sender, RoutedEventArgs e)
        {
            UpdateQuery();
        }

        private void XQueryButton_Click(object sender, RoutedEventArgs e)
        {
            var queryWindow = new ExecuteQueryWindow(_noDbService);
            queryWindow.SetQuery(new TextRange(xQuery.Document.ContentStart, xQuery.Document.ContentEnd).Text, _noDbConnectionType);
            queryWindow.Show();
        }

        void UpdateQuery()
        {
            xQuery.Document.Blocks.Clear();
            if (_noDbConnectionType == NoDbConnectionType.None) return;
            if (xTables.SelectedItems.Count == 0) return;

            var tables = xTables.SelectedItems.Cast<NoDbTable>().ToList();
            xQuery.AppendText(QueryManager.GetTableQueries(tables, _noDbConnectionType, xDropBefore.IsChecked ?? false));
        }

        
    }
}
