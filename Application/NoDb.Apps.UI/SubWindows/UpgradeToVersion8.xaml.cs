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
    /// Interaction logic for UpgradeToVersion8.xaml
    /// </summary>
    public partial class UpgradeToVersion8 : Window
    {
        public UpgradeToVersion8()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void SelectTargetFolder_Click(object sender, RoutedEventArgs e)
        {
            lblTargetFolder.Text = DisplayFolderDialog() ?? "";
        }

        private void SelectSourceFolder_Click(object sender, RoutedEventArgs e)
        {
            lblSourceFolder.Text = DisplayFolderDialog() ?? "";
        }

        private void Process_Click(object sender, RoutedEventArgs e)
        {
            TableService.UpgradeToVersion8(lblSourceFolder.Text, lblTargetFolder.Text);
            WF.MessageBox.Show("Done!");
        }

        private string DisplayFolderDialog()
        {
            var dialog = new WF.FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.Desktop;
            if (dialog.ShowDialog() != WF.DialogResult.OK) return null;
            return dialog.SelectedPath;
        }
    }
}
