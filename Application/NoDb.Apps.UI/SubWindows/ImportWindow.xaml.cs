using System;
using System.Windows;
using System.Windows.Documents;

namespace NoDb.Apps.UI.SubWindows
{
    /// <summary>
    /// Interaction logic for ImportWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        public Func<string, bool> OnImport { get; set; }

        public ImportWindow(Func<string, bool> onImport)
        {
            OnImport = onImport;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var text = new TextRange(xContent.Document.ContentStart, xContent.Document.ContentEnd).Text;
            if (OnImport(text))
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
