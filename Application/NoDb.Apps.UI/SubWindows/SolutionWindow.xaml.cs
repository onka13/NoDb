using NoDb.Business.Service.Services;
using NoDb.Data.Domain.DbModels;
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
    /// Interaction logic for SolutionWindow.xaml
    /// </summary>
    public partial class SolutionWindow : Window
    {
        NoDbSolutionService solutionService;

        public SolutionWindow() : this(null)
        {
        }

        public SolutionWindow(string solutionDir)
        {
            solutionService = solutionDir != null ? new NoDbSolutionService(solutionDir) : App.SolutionService;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            xProjects.ItemsSource = solutionService.Projects;
        }

        private void XSave_Click(object sender, RoutedEventArgs e)
        {
            solutionService.UpdateAllModules(xProjects.ItemsSource as List<NoDbProject>, false);
            System.Windows.Forms.MessageBox.Show("Saved");
        }
    }
}
