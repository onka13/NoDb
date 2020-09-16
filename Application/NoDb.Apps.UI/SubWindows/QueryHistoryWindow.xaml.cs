using NoDb.Business.Service.Services;
using NoDb.Data.Domain.DbModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace NoDb.Apps.UI.SubWindows
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class QueryHistoryWindow : Window
    {
        QueryHistoryService _queryHistoryService;

        public QueryHistoryWindow()
        {
            _queryHistoryService = new QueryHistoryService(App.SolutionService.GetSettingsFolder());
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BindHistory();
            if (xHistory.Items.Count > 0) xHistory.SelectedIndex = 0;
        }

        private void XCreateNew_Click(object sender, RoutedEventArgs e)
        {
            _queryHistoryService.New(xName.Text);
            xName.Text = "";
            BindHistory();
            xHistory.SelectedIndex = xHistory.Items.Count - 1;
            System.Windows.Forms.MessageBox.Show("Created");
        }

        private void XHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (xHistory.SelectedItem == null) return;
            xContent.Document.Blocks.Clear();
            xContent.AppendText(_queryHistoryService.GetContent(xHistory.SelectedItem.ToString()));
        }

        private void XSave_Click(object sender, RoutedEventArgs e)
        {
            if (xHistory.SelectedItem == null) return;
            var text = new TextRange(xContent.Document.ContentStart, xContent.Document.ContentEnd).Text;
            _queryHistoryService.Save(xHistory.SelectedItem.ToString(), text);
            BindHistory();
            System.Windows.Forms.MessageBox.Show("Saved");
        }

        void BindHistory()
        {
            var selected = xHistory.SelectedIndex;
            xHistory.ItemsSource = null;
            xHistory.ItemsSource = _queryHistoryService.HistoryFileNames;
            xHistory.SelectedIndex = selected;
        }

    }
}
