using System.Windows;
using System.Windows.Controls;

namespace LuceneSearchClient.Views
{ 
    public partial class PageRankView : Page
    {
    
        public PageRankView()
        {
            InitializeComponent();
        }

        private void OnLoadingRow(object sender, DataGridRowEventArgs e)
        {        
            e.Row.Header = e.Row.GetIndex();        
        }     
    }
}