using NoDb.Business.Service.Services;
using NoDb.Data.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.Windows;

namespace NoDb.Apps.UI.SubWindows
{
    /// <summary>
    /// Interaction logic for SolutionWindow.xaml
    /// </summary>
    public partial class SolutionWindow : Window
    {
        private readonly NoDbService noDbService;

        public SolutionWindow(NoDbService noDbService)
        {
            this.noDbService = noDbService ?? App.NoDbService;
            InitializeComponent();
        }

        public SolutionWindow() : this(null)
        {
        }

        public Action OnUpdated { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            xProjects.ItemsSource = noDbService.NoDbSolutionService.Projects;
        }

        private void XSave_Click(object sender, RoutedEventArgs e)
        {
            noDbService.NoDbSolutionService.SetProjects(xProjects.ItemsSource as List<NoDbProject>);
            System.Windows.Forms.MessageBox.Show("Saved");
            OnUpdated?.Invoke();
        }
    }
}
