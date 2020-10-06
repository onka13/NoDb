using NoDb.Business.Service.Templates;
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
    /// Interaction logic for NewTable.xaml
    /// </summary>
    public partial class NewTable : Window
    {
        public Func<string, string, bool> OnAdd { get; set; }

        public NewTable(Func<string, string, bool> onAdd)
        {
            OnAdd = onAdd;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            xTemplate.ItemsSource = TableTemplates.DefinedTemplates.Keys;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (OnAdd(xTableName.Text, xTemplate.SelectedItem?.ToString()))
            {
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
