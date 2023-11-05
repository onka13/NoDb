using NoDb.Business.Service.Services;
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
using WF = System.Windows.Forms;

namespace NoDb.Apps.UI.SubWindows
{
    /// <summary>
    /// Interaction logic for SplitTablesJson.xaml
    /// </summary>
    public partial class SplitTablesJson : Window
    {
        public SplitTablesJson()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void SelectTables_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WF.OpenFileDialog();
            dialog.Multiselect = false;
            if (dialog.ShowDialog() != WF.DialogResult.OK) return;
            lblTablesJsonPath.Text = dialog.FileName;
        }

        private void SelectTargetFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WF.FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.Desktop;
            if (dialog.ShowDialog() != WF.DialogResult.OK) return;
            lblTargetFolder.Text = dialog.SelectedPath;
        }

        private void Process_Click(object sender, RoutedEventArgs e)
        {
            TableService.SplitOldTableJsonFile(lblTablesJsonPath.Text, lblTargetFolder.Text);
            WF.MessageBox.Show("Done!");
        }
    }
}
