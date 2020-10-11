using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace NoDb.Apps.UI.SubWindows
{
    /// <summary>
    /// Interaction logic for ListEditor.xaml
    /// </summary>
    public partial class ListEditor : Window
    {
        dynamic _getSelected;
        dynamic _getDefaultValue;

        public ListEditor(string title)
        {
            InitializeComponent();
            xList.SelectionChanged += XList_SelectionChanged;
            Title = title;
        }

        public void InitList<T>(List<T> items,
            Func<System.Collections.IList, T> defaultValueFunc = null,
            Action<List<T>> saveAction = null,
            Action<T> onSelectionChanged = null,
            Dictionary<string, Action<T>> extraActions = null
            )
        {
            xList.ItemsSource = items;
            _getSelected = new Func<T>(() =>
            {
                return (T)xList.SelectedItem;
            });
            _getDefaultValue = defaultValueFunc ?? new Func<System.Collections.IList, T>((list) =>
            {
                return (T)Activator.CreateInstance(typeof(T));
            });
            if (saveAction == null)
            {
                xSaveButton.Visibility = Visibility.Hidden;
            }
            else
            {
                xSaveButton.Click += (sender, e) =>
                {
                    saveAction(xList.ItemsSource as List<T>);
                    Close();
                };
            }
            if (onSelectionChanged != null)
            {
                xList.SelectionChanged += (sender, e) =>
                {
                    onSelectionChanged(_getSelected());
                };
            }
            if (extraActions != null)
            {
                foreach (var action in extraActions)
                {
                    var actionButton = new Button()
                    {
                        Content = action.Key,
                        Margin = new Thickness(10, 0, 0, 0)
                    };
                    actionButton.Click += (sender, e) =>
                    {
                        action.Value(_getSelected());
                        xDetail.Refresh();
                    };
                    xActionsPanel.Children.Add(actionButton);
                }
            }

        }

        private void XList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            xDetail.SelectedObject = null;
            if (xList.SelectedItem == null) return;
            xDetail.SelectedObject = _getSelected();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var items = (xList.ItemsSource as System.Collections.IList);
            items.Add(_getDefaultValue(items));
            xList.ItemsSource = null;
            xList.ItemsSource = items;
            xList.SelectedIndex = items.Count - 1;
            XList_SelectionChanged(null, null);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (xList.SelectedItem == null) return;
            var items = (xList.ItemsSource as System.Collections.IList);
            items.RemoveAt(xList.SelectedIndex);
            xDetail.SelectedObject = null;
            xList.ItemsSource = null;
            xList.ItemsSource = items;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
