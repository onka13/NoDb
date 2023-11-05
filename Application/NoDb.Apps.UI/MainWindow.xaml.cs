using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WF = System.Windows.Forms;
using System.Windows.Input;
using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;
using System.Diagnostics;
using NoDb.Business.Service.Templates;
using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.SearchModels;
using Newtonsoft.Json.Linq;
using CoreCommon.Infrastructure.Helpers;
using MySqlX.XDevAPI.Relational;
using System.Windows.Documents;
using NoDb.Business.Service.Services;

namespace NoDb.Apps.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Clone of the selected table.
        /// </summary>
        private NoDbTable selectedTable;
        private readonly NoDbService noDbService;

        public MainWindow() : this(null)
        {
        }

        public MainWindow(NoDbService noDbService)
        {
            InitializeComponent();
            this.noDbService = noDbService ?? App.NoDbService;
        }

        #region " Events "

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = "NoDb - v" + GetType().Assembly.GetName().Version.ToString();
            #region " Columns Grid Default Columns "

            xColumns.CanUserAddRows = true;
            xColumns.CanUserDeleteRows = true;
            xColumns.CanUserResizeRows = false;
            xColumns.CanUserSortColumns = false;
            xColumns.HeadersVisibility = DataGridHeadersVisibility.All;
            xColumns.SelectionMode = DataGridSelectionMode.Single;
            xColumns.RowHeaderWidth = 30;
            xColumns.AutoGenerateColumns = false;
            xColumns.Columns.Add(new DataGridTextColumn()
            {
                Header = "Name",
                Binding = new Binding("Name"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });
            xColumns.Columns.Add(new DataGridComboBoxColumn()
            {
                Header = "DataType",
                SelectedItemBinding = new Binding("DataType"),
                ItemsSource = Enum.GetValues(typeof(NoDbDataType)),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });
            xColumns.Columns.Add(new DataGridCheckBoxColumn()
            {
                Header = "Required",
                Binding = new Binding("Required"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });
            xColumns.Columns.Add(new DataGridTextColumn()
            {
                Header = "EnumName",
                Binding = new Binding("EnumName"),
                Width = new DataGridLength(1, DataGridLengthUnitType.Star)
            });

            #endregion

            xTables.SelectionChanged += XTables_SelectionChanged;
            xColumns.SelectionChanged += XColumns_SelectionChanged;

            xColumns.ItemContainerStyle = new Style
            {
                TargetType = typeof(DataGridRow),
            };
            xColumns.ItemContainerStyle.Setters.Add(new EventSetter
            {
                Event = MouseUpEvent,
                Handler = new MouseButtonEventHandler(XColumns_MouseUp)
            });
            xColumns.PreviewKeyUp += XColumns_PreviewKey;
            xColumns.PreviewKeyDown += XColumns_PreviewKey; ;

            xMainGrid.IsEnabled = false;
            xColumnsGrid.IsEnabled = false;
            xViewMenu.IsEnabled = false;

            if (noDbService != null)
            {
                lblStatusInfo.Text = noDbService.NoDbFolder;
            }
            InitService();
        }

        bool isCtrlDown = false;
        private void XColumns_PreviewKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
            {
                e.Handled = true;
                isCtrlDown = e.IsDown;
            }
            else if (isCtrlDown && e.IsDown && (e.Key == Key.Down || e.Key == Key.Up))
            {
                e.Handled = true;

                var lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(xColumns.ItemsSource);
                if (lcv.IsAddingNew || lcv.IsEditingItem)
                {
                    return;
                }

                Action<int, int> move = (int i, int to) =>
                {
                    var temp = selectedTable.Columns[i];
                    selectedTable.Columns.RemoveAt(i);
                    selectedTable.Columns.Insert(to, temp);
                    xColumns.Items.Refresh();
                    xColumns.SelectedIndex = to;
                    xColumns.Focus();
                };

                var i = xColumns.SelectedIndex;
                if (e.Key == Key.Down && i + 1 < selectedTable.Columns.Count)
                {
                    move(i, i + 1);
                }

                if (e.Key == Key.Up && i > 0)
                {
                    move(i, i - 1);
                }
            }
        }

        public void OpenNoDbFolder()
        {
            var dialog = new WF.FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.Desktop;
            if (dialog.ShowDialog() != WF.DialogResult.OK) return;

            App.InitNoDbService(dialog.SelectedPath);
            InitService();
        }

        public void InitService()
        {
            if (noDbService == null)
            {
                ClearServiceValues();
                return;
            }

            lblStatusInfo.Text = noDbService.NoDbFolder;
            BindTables();
            xMainGrid.IsEnabled = true;
            xViewMenu.IsEnabled = true;
        }

        private void XColumns_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("selected: " + xColumns.SelectedIndex);
            if (xColumns.SelectedIndex == -1 || xColumns.Items.Count <= xColumns.SelectedIndex)
            {
                xColumnDetail.SelectedObject = null;
                return;
            }
            xColumnDetail.SelectedObject = xColumns.Items[xColumns.SelectedIndex];
        }

        private void XColumns_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Right && e.RightButton != MouseButtonState.Pressed) return;
            var row = sender as DataGridRow;
            if (row == null) return;

            xColumns.CancelEdit();

            row.IsSelected = true;
            // TODO: fix the related bugs
            var contextMenu = FindResource("xContextMenuForColumns") as ContextMenu;
            contextMenu.PlacementTarget = row;
            contextMenu.IsOpen = true;
        }

        private void XTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xTables.SelectedItem == null)
            {
                selectedTable = null;
                ClearAllRelatedWithTable();
                StaticManager.SelectedTable = null;
                return;
            }

            selectedTable = (xTables.SelectedItem as NoDbTable).JsonClone();
            xColumns.ItemsSource = selectedTable.Columns;
            xTableDetail.SelectedObject = selectedTable.Detail;
            xColumnsGrid.IsEnabled = true;
            StaticManager.SelectedTable = selectedTable;
        }

        private void NewTableButton_Click(object sender, RoutedEventArgs e)
        {
            var newTableWindow = new SubWindows.NewTable((string name, string template) =>
            {
                selectedTable = noDbService.TableService.New(name, template);
                BindTables();
                return true;
            });

            newTableWindow.ShowDialog();
        }

        private void DeleteTableButton_Click(object sender, RoutedEventArgs e)
        {
            var table = GetOriginalSelectedTable();
            if (table == null) return;
            noDbService.TableService.Delete(table);
            BindTables();
        }

        private void XMenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            // TODO: check unsaved data!
            Close();
        }

        private void IndexButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (selectedTable == null) return;
            var editor = new SubWindows.ListEditor("Indexes / Keys");
            editor.InitList(selectedTable.Indices);
            editor.ShowDialog();
        }

        private void RelationButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (selectedTable == null) return;
            var editor = new SubWindows.ListEditor("Relations");
            editor.InitList(selectedTable.Relations, onSelectionChanged: relation =>
            {
            });
            editor.ShowDialog();
        }

        private void SearchButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (selectedTable == null) return;
            var editor = new SubWindows.ListEditor("Search Items");
            editor.InitList(selectedTable.SearchItems, defaultValueFunc: (list) =>
            {
                return noDbService.SearchService.GetDefaultSearchItem(selectedTable, list as List<NoDbSearchItem>);
            }, extraActions: new Dictionary<string, Action<NoDbSearchItem>>
            {
                {"Refresh All Col.", (NoDbSearchItem searchItem) => {
                    if(searchItem == null) return;
                    searchItem.AllColumns = noDbService.SearchService.GetDefaultSearchItem(selectedTable, null).AllColumns;
                }},
                {"Add Missing Col.", (NoDbSearchItem searchItem) => {
                    if(searchItem == null) return;
                    var newColumns = noDbService.SearchService.GetDefaultSearchItem(selectedTable, null).AllColumns;
                    for (int i = 0; i < newColumns.Count; i++)
                    {
                        if(!searchItem.AllColumns.Any(x => x.Name == newColumns[i].Name))
                        {
                            searchItem.AllColumns.Add(newColumns[i]);
                        }
                    }
                }},
                {"Refresh Filter Col.", (NoDbSearchItem searchItem) => {
                    if(searchItem == null) return;
                    searchItem.Columns = noDbService.SearchService.GetDefaultSearchItem(selectedTable, null).Columns;
                }},
                {"Refresh Grid Col.", (NoDbSearchItem searchItem) => {
                    if(searchItem == null) return;
                    searchItem.DisplayedColumns = noDbService.SearchService.GetDefaultSearchItem(selectedTable, null).DisplayedColumns;
                }}
            });
            editor.ShowDialog();
        }

        private void XMenuEnum_Click(object sender, RoutedEventArgs e)
        {
            var editor = new SubWindows.ListEditor("Enum List");
            editor.InitList(noDbService.EnumService.Enums.JsonClone(), (list) =>
            {
                return EnumTemplates.Default();
            }, (list) =>
            {
                noDbService.EnumService.UpdateEnums(list);
            }, extraActions: new Dictionary<string, Action<NoDbEnumDetail>>
            {
                {"Auto Numerate", (NoDbEnumDetail item) => {
                    if(item == null || item.Items.Count < 2) return;
                    var start = item.Items.FirstOrDefault().Value;
                    for (int i = 1; i < item.Items.Count; i++)
                    {
                        item.Items[i].Value = start + i;
                    }
                }}
            });
            editor.ShowDialog();
        }

        private void XMenuRevision_Click(object sender, RoutedEventArgs e)
        {
            var window = new SubWindows.RevisionsWindow(noDbService);
            window.Show();
        }

        private void XMenuTableScripts_Click(object sender, RoutedEventArgs e)
        {
            var window = new SubWindows.TableScriptsWindow(noDbService);
            window.Show();
        }

        private void XExecuteQuery_Click(object sender, RoutedEventArgs e)
        {
            var window = new SubWindows.ExecuteQueryWindow(noDbService);
            window.SetQuery("", NoDbConnectionType.Mssql);
            window.Show();
        }

        private void XQueryHistories_Click(object sender, RoutedEventArgs e)
        {
            if (noDbService == null)
            {
                OpenNoDbFolder();
                return;
            }
            var window = new SubWindows.QueryHistoryWindow();
            window.Show();
        }

        private void XImport_Click(object sender, RoutedEventArgs e)
        {
            var window = new SubWindows.ImportFromSqlWindow(noDbService);
            window.ShowDialog();
        }

        private void XMenuSetting_Click(object sender, RoutedEventArgs e)
        {
            if (noDbService == null)
            {
                OpenNoDbFolder();
                return;
            }
            var window = new SubWindows.SettingsWindow();
            window.Show();
        }

        #endregion

        void BindTables()
        {
            ClearAllRelatedWithTable();
            var lastSelectedTableHash = selectedTable?.Hash;
            xTables.ItemsSource = null;
            xTables.ItemsSource = noDbService.TableService.Tables.OrderBy(x => x.Detail.Name).ToList();
            if (lastSelectedTableHash != null) xTables.SelectedItem = noDbService.TableService.Tables.FirstOrDefault(x => x.Hash == lastSelectedTableHash);
        }

        /// <summary>
        /// xTables stores unmodified table data
        /// </summary>
        /// <returns></returns>
        NoDbTable GetOriginalSelectedTable()
        {
            return xTables.SelectedItem as NoDbTable;
        }

        void ClearServiceValues()
        {
            xMainGrid.IsEnabled = false;
            xViewMenu.IsEnabled = false;
            xTables.ItemsSource = null;
            XTables_SelectionChanged(null, null);
        }

        void ClearAllRelatedWithTable()
        {
            xColumnsGrid.IsEnabled = false;
            xColumns.ItemsSource = null;
            xColumnDetail.SelectedObject = null;
            xTableDetail.SelectedObject = null;
        }

        void Save(object sender, RoutedEventArgs e)
        {
            xColumns.CommitEdit();
            xColumns.CancelEdit();

            var table = selectedTable;
            if (table == null) return;
            table.Columns = xColumns.ItemsSource as List<NoDbColumn>;
            table.Detail = xTableDetail.SelectedObject as NoDbTableDetail;
            noDbService.TableService.UpdateTable(table);
            BindTables();

            WF.MessageBox.Show("Saved");
        }

        private void ImportButton_Clicked(object sender, RoutedEventArgs e)
        {
            var newTableWindow = new SubWindows.ImportWindow((string json) =>
            {
                try
                {
                    var jsonData = JObject.Parse(json).ToObject<Dictionary<string, object>>();

                    var table = selectedTable;
                    if (table == null) return false;
                    var columns = table.Columns = xColumns.ItemsSource as List<NoDbColumn>;

                    foreach (var item in jsonData)
                    {
                        if (columns.Any(x => x.Name == item.Key || x.ShortName == item.Key)) continue;
                        var dataType = NoDbDataType.STRING;
                        if (item.Value is int)
                        {
                            dataType = NoDbDataType.INT;
                        }
                        else if (item.Value is float || item.Value is double)
                        {
                            dataType = NoDbDataType.FLOAT;
                        }
                        else if (item.Value is decimal)
                        {
                            dataType = NoDbDataType.DECIMAL;
                        }
                        else if (item.Value is byte)
                        {
                            dataType = NoDbDataType.BYTE;
                        }
                        else if (item.Value is long)
                        {
                            dataType = NoDbDataType.LONG;
                        }
                        else if (item.Value is bool)
                        {
                            dataType = NoDbDataType.BOOL;
                        }
                        else if (item.Value is JObject)
                        {
                            continue;
                        }
                        else
                        {
                            if (item.Value != null)
                            {
                                if (DateTime.TryParse(item.Value.ToString(), out DateTime date))
                                {
                                    dataType = NoDbDataType.DATETIME;
                                }
                            }
                        }

                        columns.Add(new NoDbColumn
                        {
                            Name = item.Key,
                            DataType = dataType,
                        });
                    }
                    xColumns.ItemsSource = null;
                    xColumns.ItemsSource = columns;
                    return true;
                }
                catch (Exception ex)
                {
                    WF.MessageBox.Show("Error:" + ex.Message);
                }
                return false;
            });

            newTableWindow.ShowDialog();
        }


        private void xMenuOpen_Click(object sender, ExecutedRoutedEventArgs e)
        {
            OpenNoDbFolder();
        }

        private void xContextMenuRemove_Click(object sender, RoutedEventArgs e)
        {
            var lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(xColumns.ItemsSource);

            if (
                xColumns.SelectedIndex < 0 ||
                xColumns.SelectedIndex >= selectedTable.Columns.Count ||
                lcv.IsAddingNew ||
                lcv.IsEditingItem
                )
            {
                return;
            }

            lcv.RemoveAt(xColumns.SelectedIndex);
        }

        private void xSplitTablesJson_Click(object sender, RoutedEventArgs e)
        {
            var window = new SubWindows.SplitTablesJson();
            window.Show();
        }

        private void xMenuRefresh_Click(object sender, ExecutedRoutedEventArgs e)
        {
            InitService();
        }
    }
}
