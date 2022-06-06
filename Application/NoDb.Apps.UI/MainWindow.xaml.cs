using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WF = System.Windows.Forms;
using System.Windows.Input;
using NoDb.Business.Service.Services;
using NoDb.Data.Domain.DbModels;
using NoDb.Data.Domain.Enums;
using System.Diagnostics;
using NoDb.Business.Service.Templates;
using NoDb.Data.Domain.Converters;
using NoDb.Data.Domain.SearchModels;
using CoreCommon.Infra.Helpers;
using System.IO;
using Newtonsoft.Json.Linq;

namespace NoDb.Apps.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NoDbService _noDbService;

        /// <summary>
        /// Clone of the selected table.
        /// </summary>
        NoDbTable _selectedTable;

        public MainWindow()
        {
            InitializeComponent();
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
                TargetType = typeof(DataGridRow)
            };
            xColumns.ItemContainerStyle.Setters.Add(new EventSetter
            {
                Event = MouseUpEvent,
                Handler = new MouseButtonEventHandler(XColumns_MouseUp)
            });

            xMainGrid.IsEnabled = false;
            xColumnsGrid.IsEnabled = false;
            xViewMenu.IsEnabled = false;

            if (!string.IsNullOrEmpty(App.Folder))
            {
                lblStatusInfo.Text = App.Folder;
                InitService(App.Folder);
            }
            BindProjects();
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

            row.IsSelected = true;
            var contextMenu = FindResource("xContextMenuForColumns") as ContextMenu;
            contextMenu.PlacementTarget = row;
            contextMenu.IsOpen = true;
        }

        private void XMenuNew_Click(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new WF.FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.Desktop;
            if (dialog.ShowDialog() != WF.DialogResult.OK) return;
            lblStatusInfo.Text = dialog.SelectedPath;

            InitService(dialog.SelectedPath);
        }

        private void XTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xTables.SelectedItem == null)
            {
                _selectedTable = null;
                ClearAllRelatedWithTable();
                StaticManager.SelectedTable = null;
                return;
            }

            _selectedTable = (xTables.SelectedItem as NoDbTable).JsonClone();
            xColumns.ItemsSource = _selectedTable.Columns;
            xTableDetail.SelectedObject = _selectedTable.Detail;
            xColumnsGrid.IsEnabled = true;
            StaticManager.SelectedTable = _selectedTable;
        }

        private void NewTableButton_Click(object sender, RoutedEventArgs e)
        {
            var newTableWindow = new SubWindows.NewTable((string name, string template) =>
            {
                _selectedTable = _noDbService.TableService.New(name, template);
                BindTables();
                return true;
            });

            newTableWindow.ShowDialog();
            refreshStaticManagerSolutions();
        }

        private void DeleteTableButton_Click(object sender, RoutedEventArgs e)
        {
            var table = GetOriginalSelectedTable();
            if (table == null) return;
            _noDbService.TableService.Delete(table);
            BindTables();
            refreshStaticManagerSolutions();
        }

        private void XMenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            // TODO: check unsaved data!
            Close();
        }

        private void IndexButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (_selectedTable == null) return;
            var editor = new SubWindows.ListEditor("Indexes / Keys");
            editor.InitList(_selectedTable.Indices);
            editor.ShowDialog();
        }

        private void RelationButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (_selectedTable == null) return;
            var editor = new SubWindows.ListEditor("Relations");
            editor.InitList(_selectedTable.Relations, onSelectionChanged: relation =>
            {
            });
            editor.ShowDialog();
        }

        private void SearchButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (_selectedTable == null) return;
            var editor = new SubWindows.ListEditor("Search Items");
            editor.InitList(_selectedTable.SearchItems, defaultValueFunc: (list) =>
            {
                return _noDbService.SearchService.GetDefaultSearchItem(_selectedTable, list as List<NoDbSearchItem>);
            }, extraActions: new Dictionary<string, Action<NoDbSearchItem>>
            {
                {"Refresh All Col.", (NoDbSearchItem searchItem) => {
                    if(searchItem == null) return;
                    searchItem.AllColumns = _noDbService.SearchService.GetDefaultSearchItem(_selectedTable, null).AllColumns;
                }},
                {"Add Missing Col.", (NoDbSearchItem searchItem) => {
                    if(searchItem == null) return;
                    var newColumns = _noDbService.SearchService.GetDefaultSearchItem(_selectedTable, null).AllColumns;
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
                    searchItem.Columns = _noDbService.SearchService.GetDefaultSearchItem(_selectedTable, null).Columns;
                }},
                {"Refresh Grid Col.", (NoDbSearchItem searchItem) => {
                    if(searchItem == null) return;
                    searchItem.DisplayedColumns = _noDbService.SearchService.GetDefaultSearchItem(_selectedTable, null).DisplayedColumns;
                }}
            });
            editor.ShowDialog();
        }

        private void XMenuEnum_Click(object sender, RoutedEventArgs e)
        {
            var editor = new SubWindows.ListEditor("Enum List");
            editor.InitList(_noDbService.EnumService.Enums.EnumList.JsonClone(), (list) =>
            {
                return EnumTemplates.Default();
            }, (list) =>
            {
                _noDbService.EnumService.Enums.EnumList = list;
                _noDbService.EnumService.Save();
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
            var window = new SubWindows.RevisionsWindow(_noDbService);
            window.Show();
        }

        private void XMenuTableScripts_Click(object sender, RoutedEventArgs e)
        {
            var window = new SubWindows.TableScriptsWindow(_noDbService);
            window.Show();
        }

        private void XExecuteQuery_Click(object sender, RoutedEventArgs e)
        {
            var window = new SubWindows.ExecuteQueryWindow(_noDbService);
            window.SetQuery("", NoDbConnectionType.Mssql);
            window.Show();
        }

        private void XQueryHistories_Click(object sender, RoutedEventArgs e)
        {
            if (App.SolutionService == null)
            {
                xSolution_Click(null, null);
                return;
            }
            var window = new SubWindows.QueryHistoryWindow();
            window.Show();
        }

        private void XImport_Click(object sender, RoutedEventArgs e)
        {
            var window = new SubWindows.ImportFromSqlWindow(_noDbService);
            window.Show();
        }

        private void XMenuSetting_Click(object sender, RoutedEventArgs e)
        {
            if (App.SolutionService == null)
            {
                xSolution_Click(null, null);
                return;
            }
            var window = new SubWindows.SettingsWindow();
            window.Show();
        }

        private void xSolution_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WF.FolderBrowserDialog();
            if (dialog.ShowDialog() != WF.DialogResult.OK) return;
            App.SolutionFolder = dialog.SelectedPath;
            App.SolutionService = new NoDbSolutionService(App.SolutionFolder);
            BindProjects();
        }

        private void XProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.SolutionFolder == null || xProjects.SelectedItem == null)
            {
                ClearServiceValues();
                return;
            }
            var project = xProjects.SelectedItem as NoDbProject;
            InitService(Path.Combine(App.SolutionFolder, project.Path, NoDbSolutionService.NODB_FOLDER_NAME));
            StaticManager.SelectedProject = project.Name;
        }

        private void XSolutionSetting_Click(object sender, RoutedEventArgs e)
        {
            if (App.SolutionService == null)
            {
                xSolution_Click(null, null);
                return;
            }
            var window = new SubWindows.SolutionWindow();
            window.OnUpdated = () =>
            {
                BindProjects();
            };
            window.Show();
        }

        #endregion

        public void InitService(string selectedPath)
        {
            _noDbService = new NoDbService(selectedPath);
            BindTables();
            xMainGrid.IsEnabled = true;
            xViewMenu.IsEnabled = true;
        }

        void BindTables()
        {
            ClearAllRelatedWithTable();
            var lastSelectedTableHash = _selectedTable?.Hash;
            xTables.ItemsSource = null;
            xTables.ItemsSource = _noDbService.TableService.Tables.OrderBy(x => x.Detail.Name).ToList();
            if (lastSelectedTableHash != null) xTables.SelectedItem = _noDbService.TableService.Tables.FirstOrDefault(x => x.Hash == lastSelectedTableHash);
        }

        void BindProjects()
        {
            if (App.SolutionService == null) return;
            var projects = App.SolutionService.GetSelectedProjects();
            var selected = xProjects.SelectedIndex;
            if (App.InitialProject != null)
            {
                selected = projects.FindIndex(x => x.Name == App.InitialProject);
                App.InitialProject = null;
            }
            xProjects.ItemsSource = null;
            xProjects.ItemsSource = projects;
            xProjects.SelectedIndex = selected != -1 ? selected : 0;
            XProjects_SelectionChanged(null, null);

            refreshStaticManagerSolutions();
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
            _noDbService = null;
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
            var table = _selectedTable;
            if (table == null) return;
            table.Columns = xColumns.ItemsSource as List<NoDbColumn>;
            table.Detail = xTableDetail.SelectedObject as NoDbTableDetail;
            _noDbService.TableService.UpdateTable(table);
            BindTables();

            WF.MessageBox.Show("Saved");
        }

        void refreshStaticManagerSolutions()
        {
            var projects = App.SolutionService.GetSelectedProjects();
            StaticManager.Solution = new NoDbSolutionModel
            {
                Projects = projects.Select(x =>
                {
                    var projectService = new NoDbService(Path.Combine(App.SolutionFolder, x.Path, NoDbSolutionService.NODB_FOLDER_NAME));
                    return new NoDbProjectModel
                    {
                        Project = x,
                        Tables = projectService.TableService.Tables,
                        NoDbEnum = projectService.EnumService.Enums
                    };
                }).ToList()
            };
        }

        private void ImportButton_Clicked(object sender, RoutedEventArgs e)
        {
            var newTableWindow = new SubWindows.ImportWindow((string json) =>
            {
                try
                {
                    var jsonData = JObject.Parse(json).ToObject<Dictionary<string, object>>();

                    var table = _selectedTable;
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
    }
}
