using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LuceneSearchClient.Views
{

    public partial class SimilatorView : Page
    {
    
        public SimilatorView()
        {
            InitializeComponent();
           
        }

        private void OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex();
        }
    }
}