using System.Windows;
using LuceneSearchClient.ViewModel;

namespace LuceneSearchClient
{

    public partial class MainWindow : Window
    {        
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }
    }
}