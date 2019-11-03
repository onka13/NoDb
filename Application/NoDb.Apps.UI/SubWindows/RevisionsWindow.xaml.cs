using NoDb.Business.Service.Services;
using NoDb.Data.Domain.Enums;
using NoDb.Data.Domain.RevisionModels;
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
using WF = System.Windows.Forms;

namespace NoDb.Apps.UI.SubWindows
{
    /// <summary>
    /// Interaction logic for RevisionsWindow.xaml
    /// </summary>
    public partial class RevisionsWindow : Window
    {
        NoDbService _noDbService;
        NoDbRevision _revision;
        NoDbConnectionType _noDbConnectionType = NoDbConnectionType.Mssql;

        public RevisionsWindow(NoDbService noDbService)
        {
            InitializeComponent();
            _noDbService = noDbService;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region " Rev Detail Grid "

            xRevisionDetail.CanUserAddRows = false;
            xRevisionDetail.CanUserDeleteRows = false;
            xRevisionDetail.CanUserResizeRows = false;
            xRevisionDetail.HeadersVisibility = DataGridHeadersVisibility.All;
            xRevisionDetail.SelectionMode = DataGridSelectionMode.Extended;
            xRevisionDetail.AutoGenerateColumns = false;
            xRevisionDetail.Columns.Add(new DataGridTextColumn
            {
                Header = "Action",
                Binding = new Binding("Action"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                IsReadOnly = true
            });
            xRevisionDetail.Columns.Add(new DataGridTextColumn()
            {
                Header = "ObjectType",
                Binding = new Binding("ObjectType"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                IsReadOnly = true
            });

            #endregion

            xRevisionList.SelectionChanged += XRevisions_SelectionChanged;
            xRevisionDetail.SelectionChanged += XRevisionDetail_SelectionChanged;
            xQueryType.ItemsSource = Enum.GetNames(typeof(NoDbConnectionType));
            xQueryType.SelectedItem = _noDbConnectionType.ToString();

            Init();
        }

        private void XRevisions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xRevisionList.SelectedItem == null)
            {
                xRevisionDetail.ItemsSource = null;
                XRevisionDetail_SelectionChanged(null, null);
                return;
            }

            _revision = _noDbService.RevisionService.ReadRevision(xRevisionList.SelectedItem.ToString());

            xRevisionDetail.ItemsSource = _revision.Revisions;
            XRevisionDetail_SelectionChanged(null, null);
        }

        private void XRevisionDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xRevisionDetail.SelectedIndex == -1 || xRevisionDetail.Items.Count <= xRevisionDetail.SelectedIndex)
            {
                xOldRevision.SelectedObject = null;
                xNewRevision.SelectedObject = null;
                XQueryType_SelectionChanged(null, null);
                return;
            }

            if (xRevisionDetail.SelectedItems.Count == 1)
            {
                var index = xRevisionDetail.SelectedIndex;
                xOldRevision.SelectedObject = _noDbService.RevisionService.GetDetailByType(_revision.Revisions[index], _revision.Revisions[index].OldValue);
                xNewRevision.SelectedObject = _noDbService.RevisionService.GetDetailByType(_revision.Revisions[index], _revision.Revisions[index].NewValue);
            }

            XQueryType_SelectionChanged(null, null);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (WF.MessageBox.Show("Are you sure to delete this revision?", "Warning", WF.MessageBoxButtons.YesNo) != WF.DialogResult.Yes)
            {
                return;
            }
            if (xRevisionList.SelectedItem == null) return;
            _noDbService.RevisionService.DeleteRevision(xRevisionList.SelectedItem.ToString());
            Init();
            WF.MessageBox.Show("Deleted");
        }

        private void XQueryButton_Click(object sender, RoutedEventArgs e)
        {
            var queryWindow = new ExecuteQueryWindow(_noDbService);
            queryWindow.SetQuery(new TextRange(xQuery.Document.ContentStart, xQuery.Document.ContentEnd).Text, _noDbConnectionType);
            queryWindow.Show();
        }

        private void XQueryType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            xQuery.Document.Blocks.Clear();
            if (xQueryType.SelectedItem == null) return;

            _noDbConnectionType = (NoDbConnectionType)Enum.Parse(typeof(NoDbConnectionType), xQueryType.SelectedItem.ToString());
            if (_noDbConnectionType == NoDbConnectionType.None) return;

            for (int i = 0; i < xRevisionDetail.SelectedItems.Count; i++)
            {
                xQuery.AppendText(_noDbService.RevisionService.GetRevisionQuery(_revision, xRevisionDetail.SelectedItems[i] as NoDbRevisionDetail, _noDbConnectionType) + "\n");
            }
        }

        void Init()
        {
            xRevisionList.ItemsSource = null;
            xRevisionList.ItemsSource = _noDbService.RevisionService.GetRevisionFiles();
            XRevisions_SelectionChanged(null, null);
        }
    }
}
